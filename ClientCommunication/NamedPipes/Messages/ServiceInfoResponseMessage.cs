// Module name: ClientCommunication
// File name: ServiceInfoResponseMessage.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Runtime.InteropServices;
using ClientCommunication.Extensions;
using ClientCommunication.NamedPipes.Messages.Memory;
using ClientCommunication.SharedMemory.Internal;
using Version = EyeTrackerStreaming.Shared.Version;

namespace ClientCommunication.NamedPipes.Messages;

internal struct ServiceInfoResponseMessage
{
    public NamedPipeMessageType MessageType => NamedPipeMessageType.ServiceInfoResponse;
    public Version Version { get; set; }
    public string SharedMemoryPath { get; set; }

    public int SerializeTo(Span<byte> span)
    {
        if (span.Length < 1024)
            throw new ArgumentException("span must have length of at least 1024 bytes");
        var layout = new ServiceInfoResponseMemoryLayout();
        layout.MessageType = NamedPipeMessageType.ServiceInfoResponse;
        layout.PackedVersion = Version.ToPackedVersion();
        layout.SharedMemoryPath = SharedMemoryPath;
        MemoryMarshal.Write(span, layout);
        return sizeof(NamedPipeMessageType) + Marshal.SizeOf<PackedVersion>() + SharedMemoryPath.Length + 1;
    }
}