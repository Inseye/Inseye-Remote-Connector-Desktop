// Module name: Mocks
// File name: RouterMock.cs
// Last edit: 2024-3-21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using EyeTrackerStreaming.Shared.NullObjects;
using EyeTrackerStreaming.Shared.Routing;

namespace Mocks;

public class RouterMock : IRouter
{
    public Func<Route, CancellationToken, Task> OnNavigateTo { get; set; } = (_, _) => Task.CompletedTask;
    public Func<Route, CancellationToken, Task> OnNavigateToStack { get; set; } = (_, _) => Task.CompletedTask;
    public Func<CancellationToken, Task> OnNavigateBack { get; set; } = _ => Task.CompletedTask;
    public bool CanNavigateBack { get; set; } = false;
    public IObservable<bool> CanNavigateBackObservable { get; set; } = new NullObservable<bool>();
    public Route CurrentRoute { get; set; } = Route.None; 
    public Task NavigateTo(Route route, CancellationToken token)
    {
        return OnNavigateTo(route, token);
    }

    public Task NavigateToStack(Route route, CancellationToken token)
    {
        return OnNavigateToStack(route, token);
    }

    public Task NavigateBack(CancellationToken token)
    {
        return OnNavigateBack(token);
    }
}