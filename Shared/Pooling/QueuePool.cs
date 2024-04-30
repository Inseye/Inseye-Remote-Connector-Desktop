// Module name: Shared
// File name: QueuePool.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Microsoft.Extensions.ObjectPool;

namespace EyeTrackerStreaming.Shared.Pooling;

public static class QueuePool<T>
{
    private static readonly IPooledObjectPolicy<Queue<T>> Policy = new PooledQueuePolicy();

    public static readonly ObjectPool<Queue<T>> Shared =
        new DefaultObjectPool<Queue<T>>(Policy, 10);

    public static ObjectPool<Queue<T>> CreateNew()
    {
        return new DefaultObjectPool<Queue<T>>(Policy, 10);
    }

    private class PooledQueuePolicy : IPooledObjectPolicy<Queue<T>>
    {
        public Queue<T> Create()
        {
            return new Queue<T>(16);
        }

        public bool Return(Queue<T> obj)
        {
            if (obj.Count > 512)
                return false;
            obj.Clear();
            return true;
        }
    }
}