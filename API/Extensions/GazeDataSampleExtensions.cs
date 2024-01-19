// Module name: API
// File name: GazeDataSampleExtensions.cs
// Last edit: 2024-2-19 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using EyeTrackerStreaming.Shared;
using RemoteConnector.Proto;

namespace API.Extensions;

internal static class GazeDataSampleExtensions
{
    public static GazeDataSample ToGazeDataSample(this GazeData gazeData)
    {
        return new GazeDataSample(millisecondsUtc: gazeData.Timestamp.TotalMilliseconds(),
            leftEyeX: gazeData.LeftX, leftEyeY: gazeData.LeftY, rightEyeX: gazeData.RightX,
            rightEyeY: gazeData.RightY, gazeData.GazeEvent.ToGazeEvent());
    }
}