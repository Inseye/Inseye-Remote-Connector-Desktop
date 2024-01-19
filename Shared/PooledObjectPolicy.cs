﻿// Module name: Shared
// File name: PooledObjectPolicy.cs
// Last edit: 2024-2-5 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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