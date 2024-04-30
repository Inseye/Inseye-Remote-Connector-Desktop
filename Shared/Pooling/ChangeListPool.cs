// Module name: Shared
// File name: ChangeListPool.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Microsoft.Extensions.ObjectPool;

namespace EyeTrackerStreaming.Shared.Pooling;

public static class ChangeListPool
{
    private static readonly IPooledObjectPolicy<ChangeList> Policy = new PooledChangeListPolicy();

    public static readonly ObjectPool<ChangeList> Shared =
        new DefaultObjectPool<ChangeList>(Policy, 10);

    public static ObjectPool<ChangeList> CreateNew()
    {
        return new DefaultObjectPool<ChangeList>(Policy, 10);
    }

    private class PooledChangeListPolicy : IPooledObjectPolicy<ChangeList>
    {
        public ChangeList Create()
        {
            return new ChangeList(16);
        }

        public bool Return(ChangeList obj)
        {
            if (obj.Count > 512)
                return false;
            obj.Clear();
            return true;
        }
    }
}