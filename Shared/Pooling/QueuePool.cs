// Module name: Shared
// File name: QueuePool.cs
// Last edit: 2024-3-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

namespace EyeTrackerStreaming.Shared.Pooling;

public class QueuePool<T>
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