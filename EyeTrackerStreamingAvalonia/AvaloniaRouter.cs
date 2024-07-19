// Module name: EyeTrackerStreamingAvalonia
// File name: AvaloniaRouter.cs
// Last edit: 2024-07-17 16:30 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Extensions;
using EyeTrackerStreaming.Shared.Routing;

namespace EyeTrackerStreamingAvalonia;

public class AvaloniaRouter : IRouter
{
    private AvaloniaSynchronizationContext AvaloniaSynchronizationContext { get; } = new();
    public bool CanNavigateBack { get; }
    public IObservable<bool> CanNavigateBackObservable { get; }
    public Route CurrentRoute { get; } = Route.None;
    public async Task NavigateTo(Route route, CancellationToken token)
    {
        await AvaloniaSynchronizationContext;
    }

    public Task NavigateToStack(Route route, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task NavigateBack(CancellationToken token)
    {
        throw new NotImplementedException();
    }
}