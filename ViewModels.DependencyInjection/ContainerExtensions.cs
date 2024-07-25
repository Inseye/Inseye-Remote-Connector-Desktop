// Module name: ViewModels.DependencyInjection
// File name: ContainerExtensions.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackingStreaming.ViewModels;
using EyeTrackingStreaming.ViewModels.Interfaces;
using SimpleInjector;

namespace ViewModels.DependencyInjection;

public static class ContainerExtensions
{
	public static Container RegisterAllViewModels(this Container container)
	{
		container.Register<ISearchViewModel, SearchViewModel>(Lifestyle.Scoped);
		container.Register<IStatusViewModel, StatusViewModel>(Lifestyle.Scoped);
		container.Register<CalibrationViewModel>(Lifestyle.Scoped);
		container.Register<ICalibrationHandler, RoutingCalibrationHandler>(Lifestyle.Scoped);
		return container;
	}
}