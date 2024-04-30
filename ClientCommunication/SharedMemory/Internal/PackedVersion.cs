// Module name: ClientCommunication
// File name: PackedVersion.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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

    public PackedVersion(uint major, uint minor, uint patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    public uint Major
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _versionMajor.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _versionMajor.WriteLittleEndian(value);
    }

    public uint Minor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _versionMinor.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _versionMinor.WriteLittleEndian(value);
    }

    public uint Patch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _versionPatch.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _versionPatch.WriteLittleEndian(value);
    }
}