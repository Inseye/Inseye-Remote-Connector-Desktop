// Module name: API
// File name: GrpcRemoteService.cs
// Last edit: 2024-3-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc. - All rights reserved.
// 
// All information contained herein is, and remains the property of
// Inseye Inc. The intellectual and technical concepts contained herein are
// proprietary to Inseye Inc. and may be covered by U.S. and Foreign Patents, patents
// in process, and are protected by trade secret or copyright law. Dissemination
// of this information or reproduction of this material is strictly forbidden
// unless prior written permission is obtained from Inseye Inc. Access to the source
// code contained herein is hereby forbidden to anyone except current Inseye Inc.
// employees, managers or contractors who have executed Confidentiality and
// Non-disclosure agreements explicitly covering such access.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using API.Extensions;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using RemoteConnector.Proto;

namespace API.Grpc;

public class GrpcRemoteService : IRemoteService, IDisposable
{
    public GrpcRemoteService(RemoteService.RemoteServiceClient remoteServiceClient, ServiceOffer offer,
        ILogger<IRemoteService> serviceLogger)
    {
        Logger = serviceLogger;
        Logger.LogTrace($"Creating new instance of {nameof(GrpcRemoteService)}");
        CompositeDisposable.Add(EyeTrackerStatusObservable = new InvokeObservable<EyeTrackerStatus>());
        CompositeDisposable.Add(GazeDataSampleObservable = new InvokeObservable<GazeDataSample>());
        var lifetimeBoundedCancellationToke = new CancellationDisposable();
        ObjectLifetimeToken = lifetimeBoundedCancellationToke.Token;
        CompositeDisposable.Add(lifetimeBoundedCancellationToke);
        RemoteService = remoteServiceClient;
        CompositeDisposable.Add(RemoteServiceStatusObservable);
        CompositeDisposable.Add(GazeDataSampleSubscriptionTracker =
            new ObservableSubscriptionTracker<GazeDataSample>(GazeDataSampleObservable));
        CompositeDisposable.Add(GazeDataSampleSubscriptionTracker.SubscribersCountObservable
            .StartWith(0)
            .Buffer(2, 1)
            .Subscribe(l =>
            {
                switch (l[0], l[1])
                {
                    case (0, 1):
                        StartEyeTrackingBackgroundTask();
                        break;
                    case (1, 0):
                        StopEyeTrackingBackgroundTask();
                        break;
                }
            }));
        CompositeDisposable.Add((CallbackDisposable) (() => EyeTrackingBackgroundTaskTokenSource.Dispose()));
        CompositeDisposable.Add(EyeTrackerStatusObservable.Subscribe(status =>
            Logger.LogInformation("Eye tracker status: {status}", status)));
        CompositeDisposable.Add(RemoteServiceStatusObservable.Subscribe(status =>
            Logger.LogInformation("Remote service status: {status}", status)));
        HostInfo = offer;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        EyeTrackingStatusBackgroundTask();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    private ILogger<IRemoteService> Logger { get; }
    private RemoteService.RemoteServiceClient RemoteService { get; }
    private InvokeObservable<EyeTrackerStatus> EyeTrackerStatusObservable { get; }
    private InvokeObservable<GazeDataSample> GazeDataSampleObservable { get; }
    private CancellationToken ObjectLifetimeToken { get; }
    private CompositeDisposable CompositeDisposable { get; } = new();

    private ObservableValue<RemoteServiceStatus> RemoteServiceStatusObservable { get; } =
        new(RemoteServiceStatus.Connected);

    private ObservableSubscriptionTracker<GazeDataSample> GazeDataSampleSubscriptionTracker { get; }

    private CancellationDisposable EyeTrackingBackgroundTaskTokenSource { get; set; } =
        new(); // don't add to composite disposables

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public IObservable<RemoteServiceStatus> ServiceStatusStream => RemoteServiceStatusObservable;
    public ServiceOffer HostInfo { get; }
    public RemoteServiceStatus ServiceStatus => RemoteServiceStatusObservable.Value;
    public EyeTrackerStatus EyeTrackerStatus { get; private set; }

    public IObservable<GazeDataSample> GazeDataStream => GazeDataSampleSubscriptionTracker;
    public IObservable<EyeTrackerStatus> EyeTrackerStatusStream => EyeTrackerStatusObservable;

    public async Task<Result> PerformCalibration(CancellationToken userToken = default)
    {
        Logger.LogInformation("Performing calibration");
        ObjectLifetimeToken.ThrowIfCancellationRequested();
        userToken.ThrowIfCancellationRequested();
        var token = ObjectLifetimeToken;
        if (userToken != default)
            token = CancellationTokenSource.CreateLinkedTokenSource(userToken, ObjectLifetimeToken).Token;

        var streamingCall = RemoteService.PerformCalibration(new CalibrationRequest(), cancellationToken: token);
        var responseStream = streamingCall.ResponseStream;
        CalibrationResponse? response = null;
        try
        {
            while (await responseStream.MoveNext().ConfigureAwait(false))
            {
                response = responseStream.Current;
                if (response?.Status != CalibrationResponse.Types.Status.Ongoing)
                    break;
            }
        }
        catch (RpcException rpcException)
        {
            if (rpcException.StatusCode == StatusCode.Unavailable)
                return new ErrorResult("Connection with remote service was broken.");
            return new ErrorResult($"Connection error. StatusCode: {rpcException.StatusCode:G}");
        }

        if (response is null)
            return new ErrorResult("Remote service has not provided any details.");
        Result result = response.Status switch
        {
            CalibrationResponse.Types.Status.Unknown => new ErrorResult(
                $"Unknown reason. {response.ErrorMessage}"),
            CalibrationResponse.Types.Status.FinishedSuccessfully => SuccessResult.Default,
            CalibrationResponse.Types.Status.FinishedFailed => new ErrorResult(
                response.ErrorMessage),
            CalibrationResponse.Types.Status.MissingSoftware => new ErrorResult(
                $"Calibration application is missing on the headset. {response.ErrorMessage}"),
            CalibrationResponse.Types.Status.Ongoing => throw new Exception(
                "Ongoing state after finishing calibration."),
            _ => throw new ArgumentOutOfRangeException()
        };
        return result;
    }

    public void Disconnect()
    {
        Dispose();
    }


    private async Task EyeTrackingStatusBackgroundTask()
    {
        try
        {
            var asyncCall = RemoteService.ObserveEyeTrackerAvailability(new ObserveEyeTrackerAvailabilityRequest(),
                cancellationToken: ObjectLifetimeToken);
            var token = ObjectLifetimeToken;
            var responseStream = asyncCall.ResponseStream;
            while (await responseStream.MoveNext(token).ConfigureAwait(false))
            {
                var current = responseStream.Current;
                if (current == null)
                    continue;
                var status = current.Status switch
                {
                    EyeTrackerAvailability.Types.Status.Available =>
                        GazeDataSampleSubscriptionTracker.SubscribersCount > 0
                            ? EyeTrackerStatus.StreamingGazeData
                            : EyeTrackerStatus.ReadyForStreaming,
                    EyeTrackerAvailability.Types.Status.Unknown => EyeTrackerStatus.Unknown,
                    EyeTrackerAvailability.Types.Status.Disconnected => EyeTrackerStatus.Disconnected,
                    EyeTrackerAvailability.Types.Status.NotCalibrated => EyeTrackerStatus.NotCalibrated,
                    EyeTrackerAvailability.Types.Status.Calibrating => EyeTrackerStatus.Calibrating,
                    EyeTrackerAvailability.Types.Status.Unavailable => EyeTrackerStatus.Unknown,
                    _ => throw new ArgumentOutOfRangeException()
                };
                Logger.LogTrace("Status: {status}", status);
                EyeTrackerStatus = status;
                EyeTrackerStatusObservable.Send(status);
            }
        }
        catch (RpcException rpcException)
        {
            if (rpcException.Status.StatusCode is StatusCode.Unavailable or StatusCode.Cancelled or StatusCode.DeadlineExceeded)
            {
                // service become become unavailable, inform subscribers that eye tracker is unknown and service is broken
                RemoteServiceStatusObservable.Value = RemoteServiceStatus.Disconnected;
                EyeTrackerStatus = EyeTrackerStatus.Unknown;
                return;
            }

            Logger.LogCritical(rpcException, $"Unhandled RpcException in {nameof(EyeTrackingStatusBackgroundTask)}");
        }
        catch (Exception exception)
        {
            Logger.LogCritical(exception, $"Unhandled exception in {nameof(EyeTrackingStatusBackgroundTask)}");
        }
        finally
        {
            RemoteServiceStatusObservable.Value = RemoteServiceStatus.Disconnected;
            EyeTrackerStatusObservable.Complete();
        }
    }

    private void StartEyeTrackingBackgroundTask()
    {
        Logger.LogInformation("Starting gaze data reading background task.");
        lock (EyeTrackingBackgroundTaskTokenSource)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            EyeTrackingGazeDataBackgroundTask(EyeTrackingBackgroundTaskTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }

    private void StopEyeTrackingBackgroundTask()
    {
        Logger.LogInformation("Stopping gaze data reading background task.");
        lock (EyeTrackingBackgroundTaskTokenSource)
        {
            EyeTrackingBackgroundTaskTokenSource.Dispose();
            EyeTrackingBackgroundTaskTokenSource = new CancellationDisposable();
        }
    }

    private async Task EyeTrackingGazeDataBackgroundTask(CancellationToken token)
    {
        try
        {
            var asyncCall = RemoteService.OpenGazeStream(new GazeDataRequest(), cancellationToken: token);
            var responseStream = asyncCall.ResponseStream;

            while (await responseStream.MoveNext(token).ConfigureAwait(false))
            {
                var current = responseStream.Current;
                if (current == null)
                    continue;
                GazeDataSampleObservable.Send(current.ToGazeDataSample());
            }
        }
        catch (RpcException rpcException)
        {
            if (rpcException.Status.StatusCode == StatusCode.Unavailable)
            {
                // service become become unavailable, inform subscribers that eye tracker has finished
                GazeDataSampleObservable.Complete();
                return;
            }

            GazeDataSampleObservable.SendError(rpcException);
        }
        catch (Exception exception)
        {
            GazeDataSampleObservable.SendError(exception);
        }
    }
}