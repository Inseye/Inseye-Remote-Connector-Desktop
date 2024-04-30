// Module name: API
// File name: GazeDataSampleExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared;
using RemoteConnector.Proto;

namespace API.Extensions;

internal static class GazeDataSampleExtensions
{
    public static GazeDataSample ToGazeDataSample(this GazeData gazeData)
    {
        return new GazeDataSample(gazeData.Timestamp.TotalMilliseconds(),
            gazeData.LeftX, gazeData.LeftY, gazeData.RightX,
            gazeData.RightY, gazeData.GazeEvent.ToGazeEvent());
    }
}