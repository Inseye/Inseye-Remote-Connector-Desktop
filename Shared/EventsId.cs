// Module name: Shared
// File name: EventsId.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Microsoft.Extensions.Logging;

namespace EyeTrackerStreaming.Shared;

public static class EventsId
{
    public static EventId ConstructorCall = new(1, nameof(ConstructorCall));
    public static EventId DisposeCall = new(2, nameof(DisposeCall));
}