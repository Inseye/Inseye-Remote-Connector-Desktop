// Module name: Mocks
// File name: RemoteServiceMock.cs
// Last edit: 2024-3-21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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
    public EyeTrackerStatus EyeTrackerStatus => EyeTrackerStatus.Connected;
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