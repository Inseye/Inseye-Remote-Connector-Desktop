// Module name: Shared
// File name: MathHelpers.cs
// Last edit: 2024-06-18 14:24 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared;

public static class MathHelpers
{
    public static readonly float DegToRad = (float) Math.PI / 180.0f;
    public static readonly float RadToDeg = (float) (180.0f / Math.PI);
}