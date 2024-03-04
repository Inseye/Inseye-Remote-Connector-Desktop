// Module name: ClientCommunication
// File name: EyeTrackerDataStruct.cs
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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ClientCommunication.Utility;
// ReSharper disable BuiltInTypeReferenceStyle

namespace ClientCommunication.SharedMemory.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct EyeTrackerDataStruct
{
    private UInt64 _time;
    private Single _leftEyeX;
    private Single _leftEyeY;
    private Single _rightEyeX;
    private Single _rightEyeY;
    private UInt32 _gazeEvent;


    public UInt64 Time
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _time.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _time.WriteLittleEndian(value);
    }

    public Single LeftEyeX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _leftEyeX.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _leftEyeX.WriteLittleEndian(value);
    }

    public Single LeftEyeY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _leftEyeY.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _leftEyeY.WriteLittleEndian(value);
    }

    public Single RightEyeX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _rightEyeX.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _rightEyeX.WriteLittleEndian(value);
    }

    public Single RightEyeY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _rightEyeY.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _rightEyeY.WriteLittleEndian(value);
    }

    public GazeEvent EyeTrackerEvent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (GazeEvent) _gazeEvent.ReadLittleEndian();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _gazeEvent.WriteLittleEndian((UInt32) value);
    }
}