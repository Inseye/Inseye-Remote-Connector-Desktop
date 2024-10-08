﻿// Module name: VRChatConnector
// File name: VrChatVector3.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace VRChatConnector.DataStructures;

public struct VrChatVector3
{
    public readonly float X;
    public readonly float Y;
    public readonly float Z;

    public VrChatVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}