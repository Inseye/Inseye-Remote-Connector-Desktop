// Module name: Shared
// File name: StringBuilderPool.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace EyeTrackerStreaming.Shared;

public static class StringBuilderPool
{
    private static readonly IPooledObjectPolicy<StringBuilder> Policy = new StringBuilderPooledObjectPolicy();

    public static readonly ObjectPool<StringBuilder> Shared =
        new DefaultObjectPool<StringBuilder>(Policy);

    public static ObjectPool<StringBuilder> CreateNew()
    {
        return new DefaultObjectPool<StringBuilder>(Policy);
    }
}