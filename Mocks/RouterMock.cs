// Module name: Mocks
// File name: RouterMock.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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