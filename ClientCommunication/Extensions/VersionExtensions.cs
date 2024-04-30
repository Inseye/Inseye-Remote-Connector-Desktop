// Module name: ClientCommunication
// File name: VersionExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using ClientCommunication.SharedMemory.Internal;
using Version = EyeTrackerStreaming.Shared.Version;

namespace ClientCommunication.Extensions;

internal static class VersionExtensions
{
    internal static PackedVersion ToPackedVersion(this Version version)
    {
        return new PackedVersion((uint) version.Major, (uint) version.Minor, (uint) version.Patch);
    }
}