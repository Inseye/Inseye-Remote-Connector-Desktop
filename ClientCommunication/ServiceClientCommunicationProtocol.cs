// Module name: ClientCommunication
// File name: ServiceClientCommunicationProtocol.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Version = EyeTrackerStreaming.Shared.Version;

namespace ClientCommunication;

public static class ServiceClientCommunicationProtocol
{
    public static readonly Version Version = new(0, 1, 0);
}