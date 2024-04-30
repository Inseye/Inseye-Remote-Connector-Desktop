// Module name: Mocks
// File name: CalibrationHandlerMock.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.ServiceInterfaces;

namespace Mocks;

public class CalibrationHandlerMock : ICalibrationHandler
{
    public Func<IRemoteService, CancellationToken, Task<Result>> OnCalibrationHandler { get; set; } =
        (_, _) => Task.FromResult((Result) SuccessResult.Default);

    public Task<Result> CalibrationHandler(IRemoteService serviceUsedToPerformCalibration, CancellationToken token)
    {
        return OnCalibrationHandler(serviceUsedToPerformCalibration, token);
    }
}