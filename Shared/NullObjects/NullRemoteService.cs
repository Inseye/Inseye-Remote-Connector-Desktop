// Module name: Shared
// File name: NullRemoteService.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.ServiceInterfaces;

namespace EyeTrackerStreaming.Shared.NullObjects;

public sealed class NullRemoteService : IRemoteService
{
    public ServiceOffer HostInfo { get; } = default;
    public RemoteServiceStatus ServiceStatus { get; } = default;
    public EyeTrackerStatus EyeTrackerStatus { get; } = default;
    public IObservable<RemoteServiceStatus> ServiceStatusStream { get; } = new NullObservable<RemoteServiceStatus>();
    public IObservable<GazeDataSample> GazeDataStream { get; } = new NullObservable<GazeDataSample>();
    public IObservable<EyeTrackerStatus> EyeTrackerStatusStream { get; } = new NullObservable<EyeTrackerStatus>();

    public Task<Result> PerformCalibration(CancellationToken userToken)
    {
        return Task.FromResult((Result) SuccessResult.Default);
    }

    public void Disconnect()
    {
    }
}