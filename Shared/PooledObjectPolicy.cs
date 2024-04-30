// Module name: Shared
// File name: PooledObjectPolicy.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Microsoft.Extensions.ObjectPool;

namespace EyeTrackerStreaming.Shared;

public sealed class PooledObjectPolicy<T> : IPooledObjectPolicy<T>
{
    private readonly Func<T> _createDelegate;
    private readonly Func<T, bool> _returnDelegate;

    public PooledObjectPolicy(Func<T> createDelegate, Func<T, bool> returnDelegate)
    {
        ArgumentNullException.ThrowIfNull(createDelegate, nameof(createDelegate));
        ArgumentNullException.ThrowIfNull(returnDelegate, nameof(returnDelegate));
        _createDelegate = createDelegate;
        _returnDelegate = returnDelegate;
    }

    public T Create()
    {
        return _createDelegate();
    }

    public bool Return(T obj)
    {
        return _returnDelegate(obj);
    }
}