// Module name: Shared
// File name: ChangeListPool.cs
// Last edit: 2024-4-3 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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