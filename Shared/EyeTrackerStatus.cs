// Module name: Shared
// File name: EyeTrackerStatus.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared;

[Flags]
public enum EyeTrackerStatus
{
    Disconnected = 0,
    ReadyForStreaming = 1 << 0,
    NotCalibrated = ReadyForStreaming | (1 << 1),
    StreamingGazeData = ReadyForStreaming | (1 << 2),
    Calibrating = ReadyForStreaming | (1 << 3),
    Unknown = -1
}

public static class EyeTrackerStatusExtensions
{
    public static bool ShouldStreamGazeData(this EyeTrackerStatus status)
    {
        return status == EyeTrackerStatus.ReadyForStreaming || status == EyeTrackerStatus.StreamingGazeData;
    }
}