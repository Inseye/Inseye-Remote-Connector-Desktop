// Module name: Shared
// File name: AuthorizedClient.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared.Authorization;

public record AuthorizedClient
{
    public required string ProcessName { get; init; }
    public required int ProcessId { get; init; }
}