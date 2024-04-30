// Module name: Shared
// File name: ObjectPoolExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Microsoft.Extensions.ObjectPool;

namespace EyeTrackerStreaming.Shared.Extensions;

public static class ObjectPoolExtensions
{
    public static PooledObjectHandle<T> GetAutoDisposing<T>(this ObjectPool<T> pool) where T : class
    {
        ArgumentNullException.ThrowIfNull(pool, nameof(pool));
        return new PooledObjectHandle<T>(pool, pool.Get());
    }

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

        public static implicit operator T(PooledObjectHandle<T> handle)
        {
            return handle.Object;
        }
    }
}