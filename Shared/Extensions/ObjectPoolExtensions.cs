// Module name: Shared
// File name: ObjectPoolExtensions.cs
// Last edit: 2024-1-29 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

namespace EyeTrackerStreaming.Shared.Extensions;

public static class ObjectPoolExtensions
{
    public readonly struct PooledObjectHandle<T> : IDisposable where T : class
    {
        private readonly ObjectPool<T>? _pool;
        public readonly T Object;
        
        public PooledObjectHandle(ObjectPool<T> pool, T pooledObject)
        {
            ArgumentNullException.ThrowIfNull(pool, nameof(pool));
            ArgumentNullException.ThrowIfNull(pooledObject, nameof(pooledObject));
            _pool = pool;
            Object = pooledObject;
        }

        public void Dispose()
        {
            _pool?.Return(Object);
        }

        public static implicit operator T(PooledObjectHandle<T> handle) => handle.Object;
    }

    public static PooledObjectHandle<T> GetAutoDisposing<T>(this ObjectPool<T> pool) where T : class
    {
        ArgumentNullException.ThrowIfNull(pool, nameof(pool));
        return new PooledObjectHandle<T>(pool, pool.Get());
    }
}