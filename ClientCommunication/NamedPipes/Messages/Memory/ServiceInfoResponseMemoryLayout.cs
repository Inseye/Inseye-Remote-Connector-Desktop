// Module name: ClientCommunication
// File name: ServiceInforResponseMemoryLayout.cs
// Last edit: 2024-2-29 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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
        set { 
            fixed(void* ptr = _backingfield) {
                Encoding.ASCII.GetBytes(value, new Span<byte>(ptr, 1008));
            }
        }
    }
}