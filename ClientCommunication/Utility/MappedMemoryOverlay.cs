// Module name: ClientCommunication
// File name: Overlay.cs
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

using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;

namespace ClientCommunication.Utility;

// source: https://blog.stephencleary.com/2023/09/memory-mapped-files-overlaid-structs.html
internal sealed unsafe class MappedMemoryOverlay : IDisposable
{
    private readonly byte* _pointer;

    private readonly MemoryMappedViewAccessor _view;

    public MappedMemoryOverlay(MemoryMappedViewAccessor view)
    {
        _view = view;
        view.SafeMemoryMappedViewHandle.AcquirePointer(ref _pointer);
    }

    public void Dispose() => _view.SafeMemoryMappedViewHandle.ReleasePointer();

    public ref T As<T>() where T : struct => ref Unsafe.AsRef<T>(_pointer);
    public ref T As<T>(long offset) where T : struct => ref Unsafe.AsRef<T>(_pointer + offset);
}