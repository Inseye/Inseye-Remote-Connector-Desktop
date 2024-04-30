// Module name: API
// File name: ProtobufGazeEventExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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