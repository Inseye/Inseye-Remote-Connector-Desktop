// Module name: Shared
// File name: CancellationTokenPool.cs
// Last edit: 2024-06-12 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Microsoft.Extensions.ObjectPool;

namespace EyeTrackerStreaming.Shared.Pooling;

public static class CancellationTokenSourcePool
{
    private static readonly IPooledObjectPolicy<CancellationTokenSource> Policy = new PooledCancellationTokenSourcePolicy();

    public static readonly ObjectPool<CancellationTokenSource> Shared =
        new DefaultObjectPool<CancellationTokenSource>(Policy, 10);

    public static ObjectPool<CancellationTokenSource> CreateNew(int maxRetained=10)
    {
        return new DefaultObjectPool<CancellationTokenSource>(Policy, maxRetained); 
    }

    private class PooledCancellationTokenSourcePolicy : IPooledObjectPolicy<CancellationTokenSource>
    {
        public CancellationTokenSource Create()
        {
            return new CancellationTokenSource();
        }

        public bool Return(CancellationTokenSource obj)
        {
            if (obj.IsCancellationRequested)
                return true;
            obj.Dispose();
            return false;
        }
    }
}