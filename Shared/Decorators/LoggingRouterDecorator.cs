// Module name: Shared
// File name: LoggingRouterDecorator.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Routing;
using Microsoft.Extensions.Logging;

namespace EyeTrackerStreaming.Shared.Decorators;

public class LoggingRouterDecorator(IRouter decorated, ILogger<LoggingRouterDecorator> logger) : IRouter
{
    public bool CanNavigateBack => decorated.CanNavigateBack;

    public IObservable<bool> CanNavigateBackObservable => decorated.CanNavigateBackObservable;

    public Route CurrentRoute => decorated.CurrentRoute;

    public Task NavigateTo(Route route, CancellationToken token)
    {
        logger.LogInformation("Navigating to {route}.", route);
        return decorated.NavigateTo(route, token);
    }

    public Task NavigateToStack(Route route, CancellationToken token)
    {
        logger.LogInformation("Navigating to {route}, stack.", route);
        return decorated.NavigateToStack(route, token);
    }

    public Task NavigateBack(CancellationToken token)
    {
        logger.LogInformation("Navigating back from {route}", CurrentRoute);
        return decorated.NavigateBack(token);
    }
}