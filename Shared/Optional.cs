// Module name: Shared
// File name: Optional.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared;

public struct Optional<T>
{
    private T _value;
    public bool ValueIsSet { get; private set; }

    public T Value
    {
        get => _value;
        set
        {
            ValueIsSet = true;
            _value = value;
        }
    }

    public static implicit operator Optional<T>(T val)
    {
        return new Optional<T>
        {
            Value = val
        };
    }
}