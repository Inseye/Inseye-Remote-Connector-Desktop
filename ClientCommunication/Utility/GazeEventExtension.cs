// Module name: ClientCommunication
// File name: GazeEventExtension.cs
// Last edit: 2024-1-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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