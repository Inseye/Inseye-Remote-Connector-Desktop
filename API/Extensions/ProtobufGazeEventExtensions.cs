// Module name: API
// File name: ProtobufGazeEventExtensions.cs
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

internal static class ProtobufGazeEventExtensions
{
    public static GazeEvent ToGazeEvent(this GazeData.Types.GazeEvent gazeEvent)
    {
        return gazeEvent switch
        {
            GazeData.Types.GazeEvent.Unknown => GazeEvent.None,
            GazeData.Types.GazeEvent.None => GazeEvent.None,
            GazeData.Types.GazeEvent.LeftBlinkOrClosed => GazeEvent.LeftEyeBlinkedOrClosed,
            GazeData.Types.GazeEvent.RightBlinkOrClosed => GazeEvent.RightEyeBlinkedOrClosed,
            GazeData.Types.GazeEvent.BothBlinkOrClosed => GazeEvent.BothEyeBlinkedOrClosed,
            GazeData.Types.GazeEvent.Saccade => GazeEvent.Saccade,
            GazeData.Types.GazeEvent.HeadsetMounted => GazeEvent.HeadsetMount,
            GazeData.Types.GazeEvent.HeadsetDismounted => GazeEvent.HeadsetDismount,
            _ => throw new ArgumentOutOfRangeException(nameof(gazeEvent), gazeEvent, null)
        };
    }
}