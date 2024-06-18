// Module name: Shared
// File name: GazeDataSample.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared;

public readonly struct GazeDataSample(
    long millisecondsUtc,
    float leftEyeX,
    float leftEyeY,
    float rightEyeX,
    float rightEyeY,
    GazeEvent gazeEvent)
{
    public readonly long MillisecondsUTC = millisecondsUtc;

    /// <summary>
    ///     Left eye pitch, radians.
    /// </summary>
    public readonly float LeftEyeX = leftEyeX;

    /// <summary>
    ///     Left eye yaw, radians.
    /// </summary>
    public readonly float LeftEyeY = leftEyeY;

    /// <summary>
    ///     Right eye pitch, radians.
    /// </summary>
    public readonly float RightEyeX = rightEyeX;

    /// <summary>
    ///     Right eye yaw, radians.
    /// </summary>
    public readonly float RightEyeY = rightEyeY;

    /// <summary>
    ///     Gaze events.
    /// </summary>
    public readonly GazeEvent GazeEvent = gazeEvent;
}