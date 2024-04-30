// Module name: Shared
// File name: IRemoteService.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Results;

namespace EyeTrackerStreaming.Shared.ServiceInterfaces;

public interface IRemoteService : IGazeDataSource
{
    public ServiceOffer HostInfo { get; }
    public RemoteServiceStatus ServiceStatus { get; }
    public EyeTrackerStatus EyeTrackerStatus { get; }
    public IObservable<RemoteServiceStatus> ServiceStatusStream { get; }
    public IObservable<EyeTrackerStatus> EyeTrackerStatusStream { get; }
    public Task<Result> PerformCalibration(CancellationToken userToken);
    public void Disconnect();
}