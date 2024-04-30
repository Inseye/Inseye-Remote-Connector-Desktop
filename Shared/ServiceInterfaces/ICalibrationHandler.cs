// Module name: Shared
// File name: ICalibrationHandler.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Results;

namespace EyeTrackerStreaming.Shared.ServiceInterfaces;

public interface ICalibrationHandler
{
    public Task<Result> CalibrationHandler(IRemoteService serviceUsedToPerformCalibration, CancellationToken token);
}