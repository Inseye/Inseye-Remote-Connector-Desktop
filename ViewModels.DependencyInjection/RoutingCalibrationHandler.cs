// Module name: ViewModels.DependencyInjection
// File name: RoutingCalibrationHandler.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using EyeTrackerStreaming.Shared.Extensions;
using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackingStreaming.ViewModels;

namespace ViewModels.DependencyInjection;

public class RoutingCalibrationHandler(IRouter router, IServiceProvider serviceProvider) : ICalibrationHandler
{
	public async Task<Result> CalibrationHandler(IRemoteService serviceUsedToPerformCalibration,
		CancellationToken token)
	{
		try
		{
			await router.NavigateToStack(Route.Calibration, token);
			var viewModel = serviceProvider.GetServiceRequired<CalibrationViewModel>();
			return await viewModel.Calibrate(serviceUsedToPerformCalibration, token);
		}
		finally
		{
			await router.NavigateBack(default);
		}
	}
}