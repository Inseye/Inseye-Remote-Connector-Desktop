// Module name: Shared
// File name: SynchronizationContextExtensions.cs
// Last edit: 2024-07-18 10:05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Runtime.CompilerServices;

namespace EyeTrackerStreaming.Shared.Extensions;

public static class SynchronizationContextExtensions
{
    public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext context)
    {
        return new SynchronizationContextAwaiter(context);
    }
}

public readonly struct SynchronizationContextAwaiter : INotifyCompletion

{
    private static readonly SendOrPostCallback PostCallback = state => ((Action) state)();

    private readonly SynchronizationContext _context;

    public SynchronizationContextAwaiter(SynchronizationContext context)

    {
        _context = context;
    }

    public bool IsCompleted => _context == SynchronizationContext.Current;

    public void OnCompleted(Action continuation)

    {
        _context.Post(PostCallback, continuation);
    }

    public void GetResult()

    {
    }
}