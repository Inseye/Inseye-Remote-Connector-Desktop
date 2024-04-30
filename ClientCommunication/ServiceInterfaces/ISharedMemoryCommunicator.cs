// Module name: ClientCommunication
// File name: ISharedMemoryCommunicator.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.ServiceInterfaces;

namespace ClientCommunication.ServiceInterfaces;

public interface ISharedMemoryCommunicator : IGazeDataSink, IDisposable
{
    string SharedMemoryFilePath { get; }
}