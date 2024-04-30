// Module name: ClientCommunication
// File name: ServiceInfoResponseMemoryLayout.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Runtime.InteropServices;
using System.Text;
using ClientCommunication.SharedMemory.Internal;

namespace ClientCommunication.NamedPipes.Messages.Memory;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
internal unsafe struct ServiceInfoResponseMemoryLayout
{
    public NamedPipeMessageType MessageType;
    public PackedVersion PackedVersion;
    private fixed byte _backingfield[1008];

    public string SharedMemoryPath
    {
        set
        {
            fixed (void* ptr = _backingfield)
            {
                Encoding.ASCII.GetBytes(value, new Span<byte>(ptr, 1008));
            }
        }
    }
}