// Module name: Mocks
// File name: RemoteServiceMock.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Diagnostics;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using Version = EyeTrackerStreaming.Shared.Version;

namespace Mocks;

public sealed class RemoteServiceMock : IRemoteService, IDisposable
{
    private readonly CancellationTokenSource _lifetimeTokenSource = new();
    private readonly InvokeObservable<EyeTrackerStatus> _eyeTrackerStatus = new();
    private readonly InvokeObservable<GazeDataSample> _gazeDataStream = new();
    private readonly InvokeObservable<RemoteServiceStatus> _remoteStatusStream = new();

    public static RemoteServiceMock Default =>
        new(new ServiceOffer("mock service", "0.0.0.0", 1234, new Version(0, 0, 1, "mock")));

    public RemoteServiceMock(ServiceOffer serviceOffer)
    {
        HostInfo = serviceOffer;
        BackgroundDataGenerator();
    }

    private async void BackgroundDataGenerator()
    {
        var token = _lifetimeTokenSource.Token;
        while (!token.IsCancellationRequested)
        {
            var span = DateTime.UtcNow - DateTime.UnixEpoch;
            var ms = span.TotalMilliseconds;
            var x = Math.Sin((ms / 5000.0d) * 2 * Math.PI);
            x *= Math.Sign(x) * x * 30 * MathHelpers.DegToRad;
            var y = Math.Cos((ms / 5000.0d) * 2 * Math.PI);
            y *= Math.Sign(y) * y * 30 * MathHelpers.DegToRad;
            var @event = GazeEvent.BothEyeBlinkedOrClosed;
            if (((long)ms) % 2000 - 1000 > 0)
            {
                @event = GazeEvent.HeadsetMount;
            }
            _gazeDataStream.Send(new GazeDataSample((long) ms,  (float) x, (float) y, (float) x, (float) y, @event));
            // _gazeDataStream.Send(new GazeDataSample((long) ms,  (float) 0.0f, (float) 0.0f, (float) 0.0f, (float) 0.0f, @event));
            await Task.Delay(100, token);
        }
    }
    public void Dispose()
    {
        _remoteStatusStream.Dispose();
        _gazeDataStream.Dispose();
        _eyeTrackerStatus.Dispose();
        _lifetimeTokenSource.Cancel();
        _lifetimeTokenSource.Dispose();
    }


    public ServiceOffer HostInfo { get; }
    public RemoteServiceStatus ServiceStatus => RemoteServiceStatus.Connected;
    public EyeTrackerStatus EyeTrackerStatus => EyeTrackerStatus.ReadyForStreaming;
    public IObservable<RemoteServiceStatus> ServiceStatusStream => _remoteStatusStream;
    public IObservable<GazeDataSample> GazeDataStream => _gazeDataStream;
    public IObservable<EyeTrackerStatus> EyeTrackerStatusStream => _eyeTrackerStatus;

    public async Task<Result> PerformCalibration(CancellationToken userToken)
    {
        await Task.Delay(4000, userToken);
        return SuccessResult.Default;
    }

    public void Disconnect()
    {
    }
}