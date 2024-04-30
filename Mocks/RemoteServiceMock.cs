// Module name: Mocks
// File name: RemoteServiceMock.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using Version = EyeTrackerStreaming.Shared.Version;

namespace Mocks;

public sealed class RemoteServiceMock(ServiceOffer offer) : IRemoteService, IDisposable
{
    private readonly InvokeObservable<EyeTrackerStatus> _eyeTrackerStatus = new();
    private readonly InvokeObservable<GazeDataSample> _gazeDataStream = new();
    private readonly InvokeObservable<RemoteServiceStatus> _remoteStatusStream = new();

    public static RemoteServiceMock Default =>
        new(new ServiceOffer("mock service", "0.0.0.0", 1234, new Version(0, 0, 1, "mock")));

    public void Dispose()
    {
        _remoteStatusStream.Dispose();
        _gazeDataStream.Dispose();
        _eyeTrackerStatus.Dispose();
    }

    public ServiceOffer HostInfo { get; } = offer;
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