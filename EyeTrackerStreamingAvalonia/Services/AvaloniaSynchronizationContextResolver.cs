// Module name: EyeTrackerStreamingAvalonia
// File name: AvaloniaSynchronizationContextResolver.cs
// Last edit: 2024-07-26 15:06 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Threading;
using Avalonia.Threading;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using SimpleInjector;

namespace EyeTrackerStreamingAvalonia.Services;

internal sealed class AvaloniaSynchronizationContextResolver : IUiThreadSynchronizationContext
{
    public AvaloniaSynchronizationContextResolver(Container container)
    {
        if (container.IsVerifying)
            Context = new SynchronizationContext(); // implement some kind of disposing context
        else
            Context = new AvaloniaSynchronizationContext();

    }

    public SynchronizationContext Context { get; }
}