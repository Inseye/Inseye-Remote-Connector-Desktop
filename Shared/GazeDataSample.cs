// Module name: Shared
// File name: GazeDataSample.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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
    public readonly float LeftEyeX = leftEyeX;
    public readonly float LeftEyeY = leftEyeY;
    public readonly float RightEyeX = rightEyeX;
    public readonly float RightEyeY = rightEyeY;
    public readonly GazeEvent GazeEvent = gazeEvent;
}