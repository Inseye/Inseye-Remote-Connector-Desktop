// Module name: ClientCommunication
// File name: GazeEvent.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

// ReSharper disable BuiltInTypeReferenceStyle

namespace ClientCommunication.SharedMemory.Internal;

internal enum GazeEvent : Int32
{
    None = 0,
    BlinkLeft = 1 << 0,
    BlinkRight = 1 << 1,
    BlinkBoth = 1 << 2,
    Saccade = 1 << 3,
    HeadsetMount = 1 << 4,
    HeadsetDismount = 1 << 5
}