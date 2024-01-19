// Module name: Shared
// File name: ThreadSafeBool.cs
// Last edit: 2024-1-31 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Runtime.CompilerServices;

namespace EyeTrackerStreaming.Shared;

public struct DisposeBool : IEquatable<bool>
{
    private int _backingField;

    private bool Disposed => _backingField == 1;
    
    /// <summary>
    /// Checks dispose flag and sets it if not set.
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

    public static implicit operator bool(DisposeBool dis) => dis.Disposed;

    public static implicit operator DisposeBool(bool val) => new()
    {
        _backingField = val ? 1 : 0
    };
}