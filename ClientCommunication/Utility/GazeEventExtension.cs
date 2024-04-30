// Module name: ClientCommunication
// File name: GazeEventExtension.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using GazeEvent = EyeTrackerStreaming.Shared.GazeEvent;

namespace ClientCommunication.Utility;

internal static class GazeEventExtension
{
    internal static SharedMemory.Internal.GazeEvent ToEyeTrackerEvent(this GazeEvent @event)
    {
        return @event switch
        {
            GazeEvent.None => SharedMemory.Internal.GazeEvent.None,
            GazeEvent.LeftEyeBlinkedOrClosed => SharedMemory.Internal.GazeEvent.BlinkLeft,
            GazeEvent.RightEyeBlinkedOrClosed => SharedMemory.Internal.GazeEvent.BlinkRight,
            GazeEvent.BothEyeBlinkedOrClosed => SharedMemory.Internal.GazeEvent.BlinkBoth,
            GazeEvent.Saccade => SharedMemory.Internal.GazeEvent.Saccade,
            GazeEvent.HeadsetMount => SharedMemory.Internal.GazeEvent.HeadsetMount,
            GazeEvent.HeadsetDismount => SharedMemory.Internal.GazeEvent.HeadsetDismount,
            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null)
        };
    }
}