﻿// Module name: Shared
// File name: BoolToggle.cs
// Last edit: 2024-4-9 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

namespace EyeTrackerStreaming.Shared.Structs;

/// <summary>
/// When constructed switches flag to opposite value.
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
    /// Switches flag back to original value.
    /// </summary>
    public void Dispose()
    {
        _flag = _initialValue;
    }
}