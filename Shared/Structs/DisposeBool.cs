// Module name: Shared
// File name: DisposeBool.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Runtime.CompilerServices;

namespace EyeTrackerStreaming.Shared.Structs;

public struct DisposeBool : IEquatable<bool>
{
    private int _backingField;

    private bool Disposed => _backingField == 1;

    /// <summary>
    ///     Checks dispose flag and sets it if not set.
    /// </summary>
    /// <returns>true if dispose operation should be performed immediately</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PerformDispose()
    {
        return Interlocked.CompareExchange(ref _backingField, 1, 0) == 0;
    }

    public bool Equals(bool other)
    {
        return other == Disposed;
    }

    public static implicit operator bool(DisposeBool dis)
    {
        return dis.Disposed;
    }

    public static implicit operator DisposeBool(bool val)
    {
        return new DisposeBool
        {
            _backingField = val ? 1 : 0
        };
    }
}