// Module name: Shared
// File name: Ref.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared;

/// <summary>
///     Explicitly boxed struct
/// </summary>
/// <typeparam name="T">Type of struct to box</typeparam>
public sealed class Ref<T> where T : struct
{
    public Ref(T initialValue)
    {
        Value = initialValue;
    }

    public T Value { get; set; }
}