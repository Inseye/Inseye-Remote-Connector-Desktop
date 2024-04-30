// Module name: Shared
// File name: ObjectExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared.Extensions;

public static class ObjectExtensions
{
    public static T Apply<T>(this T @this, Action<T> func)
    {
        func(@this);
        return @this;
    }
}