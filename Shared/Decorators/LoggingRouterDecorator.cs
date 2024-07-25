// Module name: Shared
// File name: LoggingRouterDecorator.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using EyeTrackerStreaming.Shared.Routing;
using Microsoft.Extensions.Logging;

namespace EyeTrackerStreaming.Shared.Decorators;

public class LoggingRouterDecorator(IRouter decorated, ILogger<LoggingRouterDecorator> logger) : IRouter
{
	public bool CanNavigateBack => decorated.CanNavigateBack;

	public IObservable<bool> CanNavigateBackObservable => decorated.CanNavigateBackObservable;

	public Route CurrentRoute => decorated.CurrentRoute;

	public Task NavigateTo(Route route, CancellationToken token, object context = null)
	{
		logger.LogInformation("Navigating to {route}.", route);
		return decorated.NavigateTo(route, token, context);
	}

	public Task NavigateToStack(Route route, CancellationToken token, object context = null)
	{
		logger.LogInformation("Navigating to {route}, stack.", route);
		return decorated.NavigateToStack(route, token, context);
	}

	public Task NavigateBack(CancellationToken token, object context = null)
	{
		logger.LogInformation("Navigating back from {route}", CurrentRoute);
		return decorated.NavigateBack(token, context);
	}
}