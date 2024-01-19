// Module name: ClientCommunication
// File name: SharedMemoryHeader.cs
// Last edit: 2024-1-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

// ReSharper disable BuiltInTypeReferenceStyle

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ClientCommunication.Utility;

namespace ClientCommunication.SystemInterop.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct SharedMemoryHeader
{
    private UInt32 _versionMajor;
    private UInt32 _versionMinor;
    private UInt32 _versionPatch;
    private UInt32 _headerSize;
    private UInt32 _totalSize;
    private UInt32 _sample_size;
    private volatile UInt32 _samplesWritten;

    public UInt32 VersionMajor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _versionMajor.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _versionMajor.WriteLittleEndian(value);
    }

    public UInt32 VersionMinor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _versionMinor.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _versionMinor.WriteLittleEndian(value);
    }

    public UInt32 VersionPatch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _versionPatch.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _versionPatch.WriteLittleEndian(value);
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