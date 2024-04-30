// Module name: ClientCommunication
// File name: SharedMemoryHeader.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

// ReSharper disable BuiltInTypeReferenceStyle

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ClientCommunication.Utility;

namespace ClientCommunication.SharedMemory.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct SharedMemoryHeader
{
    private PackedVersion _version;
    private UInt32 _headerSize;
    private UInt32 _totalSize;
    private UInt32 _sample_size;
    private volatile UInt32 _samplesWritten;

    public UInt32 VersionMajor
    {
        get => _version.Major;
        set => _version.Major = value;
    }

    public UInt32 VersionMinor
    {
        get => _version.Minor;
        set => _version.Minor = value;
    }

    public UInt32 VersionPatch
    {
        get => _version.Patch;
        set => _version.Patch = value;
    }

    public UInt32 HeaderSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _headerSize.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _headerSize.WriteLittleEndian(value);
    }

    public UInt32 TotalSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _totalSize.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _totalSize.WriteLittleEndian(value);
    }

    public UInt32 SampleSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _sample_size.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _sample_size.WriteLittleEndian(value);
    }

    public UInt32 SamplesWritten
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _samplesWritten.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _samplesWritten = value.ReadLittleEndian();
    }
}