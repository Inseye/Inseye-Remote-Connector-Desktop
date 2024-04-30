// Module name: EyeTrackerStreamingConsole
// File name: NullGazeDataSink.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;

namespace EyeTrackerStreamingConsole.Services;

public class NullGazeDataSink : IGazeDataSink
{
    public void WriteGazeData(in GazeDataSample gazeDataSample)
    {
    }
}