// Module name: ClientCommunication
// File name: ServiceInfo.cs
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