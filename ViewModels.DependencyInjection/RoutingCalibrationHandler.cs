// Module name: ViewModels.DependencyInjection
// File name: RoutingCalibrationHandler.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackingStreaming.ViewModels;
using Shared.DependencyInjection.Interfaces;

namespace ViewModels.DependencyInjection;

public class RoutingCalibrationHandler(IScopingRouter router) : ICalibrationHandler
{
    public async Task<Result> CalibrationHandler(IRemoteService serviceUsedToPerformCalibration,
        CancellationToken token)
    {
        try
        {
            await router.NavigateToStack(Route.Calibration, token);
            var viewModel = router.CurrentRouteScope.GetInstance<CalibrationViewModel>();
            return await viewModel.Calibrate(serviceUsedToPerformCalibration, token);
        }
        finally
        {
            await router.NavigateBack(default);
        }
    }
}