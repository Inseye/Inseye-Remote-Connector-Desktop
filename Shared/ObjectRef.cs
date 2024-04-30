// Module name: Shared
// File name: ObjectRef.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared;

public sealed class ObjectRef<T> where T : class
{
    public T Object { get; set; }

    public static explicit operator ObjectRef<T>(T referenced)
    {
        return new ObjectRef<T>
        {
            Object = referenced
        };
    }

    public static implicit operator T(ObjectRef<T> reference)
    {
        return reference.Object;
    }
}