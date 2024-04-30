// Module name: Shared
// File name: BoolToggle.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared.Structs;

/// <summary>
///     When constructed switches flag to opposite value.
/// </summary>
public readonly ref struct BoolToggle
{
    private readonly ref bool _flag;
    private readonly bool _initialValue;

    public BoolToggle(ref bool flag)
    {
        _initialValue = flag;
        _flag = ref flag;
        _flag = !_flag;
    }

    /// <summary>
    ///     Switches flag back to original value.
    /// </summary>
    public void Dispose()
    {
        _flag = _initialValue;
    }
}