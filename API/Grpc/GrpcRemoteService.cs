// Module name: API
// File name: GrpcRemoteService.cs
// Last edit: 2024-1-31 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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
using EyeTrackerStreaming.Shared.Extensions;
using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using RemoteConnector.Proto;

namespace API.Grpc;

public class GrpcRemoteService : IRemoteService, IDisposable
{
    private readonly ILogger<GrpcRemoteService> _logger;
    private readonly RemoteService.RemoteServiceClient _remoteService;
    private readonly InvokeObservable<EyeTrackerStatus> _eyeTrackerStatusObservable;
    private readonly InvokeObservable<GazeDataSample> _gazeDataSampleObservable;
    private readonly CancellationToken _objectLifetimeToken;
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ObservableValue<RemoteServiceStatus> _remoteServiceStatusObservable = new(RemoteServiceStatus.Connected);
    private readonly ObservableSubscriptionTracker<GazeDataSample> _gazeDataSampleSubscriptionTracker;
    private CancellationDisposable _eyeTrackingBackgroundTaskTokenSource = new(); // don't add to composite disposables

    public GrpcRemoteService(Channel openChannel, ServiceOffer offer, ILogger<GrpcRemoteService> serviceLogger)
    {
        _logger = serviceLogger;
        _logger.LogTrace($"Creating new instance of {nameof(GrpcRemoteService)}");
        _compositeDisposable.Add(_eyeTrackerStatusObservable = new());
        _compositeDisposable.Add(_gazeDataSampleObservable = new ());
        var lifetimeBoundedCancellationToke = new CancellationDisposable();
        _objectLifetimeToken = lifetimeBoundedCancellationToke.Token;
        _compositeDisposable.Add(lifetimeBoundedCancellationToke);
        _remoteService = new RemoteService.RemoteServiceClient(openChannel);
        _compositeDisposable.Add(_remoteServiceStatusObservable);
        _compositeDisposable.Add(_gazeDataSampleSubscriptionTracker = new ObservableSubscriptionTracker<GazeDataSample>(_gazeDataSampleObservable));
        _compositeDisposable.Add(_gazeDataSampleSubscriptionTracker.SubscribersCountObservable
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
        _compositeDisposable.Add((CallbackDisposable)(() => _eyeTrackingBackgroundTaskTokenSource.Dispose()));
        _compositeDisposable.Add(_eyeTrackerStatusObservable.Subscribe(status => _logger.LogInformation("Eye tracker status: {status}", status)));
        _compositeDisposable.Add(_remoteServiceStatusObservable.Subscribe(status => _logger.LogInformation("Remote service statusL {status}", status)));
        HostInfo = offer;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        EyeTrackingStatusBackgroundTask();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    public IObservable<RemoteServiceStatus> ServiceStatusStream => _remoteServiceStatusObservable;
    public ServiceOffer HostInfo { get; }
    public RemoteServiceStatus ServiceStatus => _remoteServiceStatusObservable.Value;

    public IObservable<GazeDataSample> GazeDataStream => _gazeDataSampleSubscriptionTracker;
    public IObservable<EyeTrackerStatus> EyeTrackerStatusStream => _eyeTrackerStatusObservable;

    public async Task<Result> PerformCalibration(CancellationToken userToken = default)
    {
        _logger.LogInformation("Performing calibration");
        _objectLifetimeToken.ThrowIfCancellationRequested();
        userToken.ThrowIfCancellationRequested();
        var token = _objectLifetimeToken;
        if (userToken != default)
        {
            token = CancellationTokenSource.CreateLinkedTokenSource(userToken, _objectLifetimeToken).Token;
        }

        var streamingCall = _remoteService.PerformCalibration(new CalibrationRequest(), cancellationToken: token);
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
                return new ErrorResult("Calibration failed, Connection with remote service was broken.");
            throw;
        }

        if (response is null)
            return new ErrorResult("Calibration failed. Remote service has not provided any details.");
        Result result = response.Status switch
        {
            CalibrationResponse.Types.Status.Unknown => new ErrorResult(
                "Calibration failed with unknown reason.".ConcatStrings(response.ErrorMessage)),
            CalibrationResponse.Types.Status.FinishedSuccessfully => SuccessResult.Default,
            CalibrationResponse.Types.Status.FinishedFailed => new ErrorResult(
                "Calibration failed.".ConcatStrings(response.ErrorMessage)),
            CalibrationResponse.Types.Status.MissingSoftware => new ErrorResult(
                "Calibration application is missing on the headset.".ConcatStrings(response.ErrorMessage)),
            CalibrationResponse.Types.Status.Ongoing => throw new Exception(
                "Ongoing state after finishing calibration."),
            _ => throw new ArgumentOutOfRangeException()
        };
        return result;
    }
    

    private async Task EyeTrackingStatusBackgroundTask()
    {
        try
        {
            var asyncCall = _remoteService.ObserveEyeTrackerAvailability(new ObserveEyeTrackerAvailabilityRequest(),
                cancellationToken: _objectLifetimeToken);
            var token = _objectLifetimeToken;
            var responseStream = asyncCall.ResponseStream;

            while (await responseStream.MoveNext(token).ConfigureAwait(false))
            {
                var current = responseStream.Current;
                if (current == null)
                    continue;
                var status = current.Status switch
                {
                    EyeTrackerAvailability.Types.Status.Available => _gazeDataSampleSubscriptionTracker.SubscribersCount > 0
                        ? EyeTrackerStatus.StreamingGazeData
                        : EyeTrackerStatus.Connected,
                    EyeTrackerAvailability.Types.Status.Unknown => EyeTrackerStatus.Unknown,
                    EyeTrackerAvailability.Types.Status.Disconnected => EyeTrackerStatus.Disconnected,
                    EyeTrackerAvailability.Types.Status.NotCalibrated => EyeTrackerStatus.NotCalibrated,
                    EyeTrackerAvailability.Types.Status.Calibrating => EyeTrackerStatus.Calibrating,
                    EyeTrackerAvailability.Types.Status.Unavailable => EyeTrackerStatus.Unknown,
                    _ => throw new ArgumentOutOfRangeException()
                };
                // TODO: consider pushing this in async manner
                _eyeTrackerStatusObservable.Send(status);
            }
        }
        catch (RpcException rpcException)
        {
            if (rpcException.Status.StatusCode == StatusCode.Unavailable)
            {
                // service become become unavailable, inform subscribers that eye tracker is unknown and service is broken
                _remoteServiceStatusObservable.Value = RemoteServiceStatus.Disconnected;
                _eyeTrackerStatusObservable.Send(EyeTrackerStatus.Unknown);
                _eyeTrackerStatusObservable.Complete();
                return;
            }

            throw;
        }
        catch (Exception exception)
        {
            // TODO: Consider what to do with observers exceptions thrown during 'SendError' invoke
            _eyeTrackerStatusObservable.SendError(exception);
        }
        finally
        {
            _eyeTrackerStatusObservable.Complete();
        }
    }

    private void StartEyeTrackingBackgroundTask()
    {
        _logger.LogInformation("Starting gaze data reading background task.");
        lock (_eyeTrackingBackgroundTaskTokenSource)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            EyeTrackingGazeDataBackgroundTask(_eyeTrackingBackgroundTaskTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }

    private void StopEyeTrackingBackgroundTask()
    {
        _logger.LogInformation("Stopping gaze data reading background task.");
        lock (_eyeTrackingBackgroundTaskTokenSource)
        {
            _eyeTrackingBackgroundTaskTokenSource.Dispose();
            _eyeTrackingBackgroundTaskTokenSource = new();
        }
    }

    private async Task EyeTrackingGazeDataBackgroundTask(CancellationToken token)
    {
        try
        {
            var asyncCall = _remoteService.OpenGazeStream(new GazeDataRequest(), cancellationToken: token);
            var responseStream = asyncCall.ResponseStream;

            while (await responseStream.MoveNext(token).ConfigureAwait(false))
            {
                var current = responseStream.Current;
                if (current == null)
                    continue;
                _gazeDataSampleObservable.Send(current.ToGazeDataSample());
            }
        }
        catch (RpcException rpcException)
        {
            if (rpcException.Status.StatusCode == StatusCode.Unavailable)
            {
                // service become become unavailable, inform subscribers that eye tracker has finished
                _gazeDataSampleObservable.Complete();
                return;
            }

            throw;
        }
        catch (Exception exception)
        {
            _gazeDataSampleObservable.SendError(exception);
        }
        finally
        {
            _gazeDataSampleObservable.Complete();
        }
    }
    
    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
}