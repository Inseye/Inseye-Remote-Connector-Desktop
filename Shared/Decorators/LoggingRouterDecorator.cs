// Module name: Shared
// File name: RouterDecorator.cs
// Last edit: 2024-2-1 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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