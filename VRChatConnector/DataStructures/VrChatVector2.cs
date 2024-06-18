// Module name: VRChatConnector
// File name: VrChatVector2.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace VRChatConnector.DataStructures;

internal readonly struct VrChatVector2
{
    public readonly float X;
    public readonly float Y;

    public VrChatVector2(float x, float y)
    {
        X = x;
        Y = y;
    }
}