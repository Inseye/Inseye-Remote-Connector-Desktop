// Module name: Shared
// File name: CallbackDisposable.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Structs;

namespace EyeTrackerStreaming.Shared;

public sealed class CallbackDisposable : IDisposable
{
    private readonly Action _actionToInvokeOnDispose;
    private DisposeBool _dispose;

    public CallbackDisposable(Action actionToInvokeOnDispose)
    {
        ArgumentNullException.ThrowIfNull(actionToInvokeOnDispose, nameof(actionToInvokeOnDispose));
        _actionToInvokeOnDispose = actionToInvokeOnDispose;
    }

    public void Dispose()
    {
        if (_dispose.PerformDispose())
            _actionToInvokeOnDispose.Invoke();
    }

    public static explicit operator CallbackDisposable(Action action)
    {
        return new CallbackDisposable(action);
    }
}