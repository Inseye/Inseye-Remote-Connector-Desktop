// Module name: ClientCommunication
// File name: PackedVersion.cs
// Last edit: 2024-2-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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
using System.Runtime.InteropServices;
using ClientCommunication.Utility;

namespace ClientCommunication.SharedMemory.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PackedVersion
{
    private UInt32 _versionMajor;
    private UInt32 _versionMinor;
    private UInt32 _versionPatch;

    public PackedVersion(UInt32 major, UInt32 minor, UInt32 patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }
    
    public UInt32 Major
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _versionMajor.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _versionMajor.WriteLittleEndian(value);
    }

    public UInt32 Minor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _versionMinor.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _versionMinor.WriteLittleEndian(value);
    }

    public UInt32 Patch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _versionPatch.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _versionPatch.WriteLittleEndian(value);
    }
}