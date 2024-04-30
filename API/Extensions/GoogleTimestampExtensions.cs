// Module name: API
// File name: GoogleTimestampExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Google.Protobuf.WellKnownTypes;

namespace API.Extensions;

internal static class GoogleTimestampExtensions
{
    public static long TotalMilliseconds(this Timestamp timestamp)
    {
        return timestamp.Seconds * 1000L + timestamp.Nanos / 1_000_000L;
    }
}