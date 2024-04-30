// Module name: ClientCommunication
// File name: MappedMemoryOverlay.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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

    public void Dispose()
    {
        _view.SafeMemoryMappedViewHandle.ReleasePointer();
    }

    public ref T As<T>() where T : struct
    {
        return ref Unsafe.AsRef<T>(_pointer);
    }

    public ref T As<T>(long offset) where T : struct
    {
        return ref Unsafe.AsRef<T>(_pointer + offset);
    }
}