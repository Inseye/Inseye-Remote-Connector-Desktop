// Module name: ClientCommunicationTester
// File name: Program.cs
// Last edit: 2024-3-21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using ClientCommunication.NamedPipes;
using ClientCommunication.ServiceInterfaces;
using ClientCommunication.SharedMemory;
using ClientCommunication.Utility;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Utility;
using Microsoft.Extensions.Logging.Abstractions;

// Console.WriteLine(SharedMemoryCommunicator.TotalSize);
// return;

NewMain();

return;

static void NewMain()
{
    var factory = new SharedMemoryFactory(SimpleConsoleLogger<ISharedMemoryCommunicator>.Instance);
    var pipeServer = new NamedPipeServer(factory, SimpleConsoleLogger<NamedPipeServer>.Instance);
    pipeServer.WaitForServeLoopClose().Wait();
}

static void OldMain()
{
    using var comm = new SharedMemoryCommunicator("Local\\Inseye-Remote-Connector-Shared-Memory",
        NullLogger<SharedMemoryCommunicator>.Instance);
    GazeDataSample sample;
    var i = 1;
    while (i > 0)
    {
        sample = new GazeDataSample(DateTime.UtcNow.ToBinary(), i, i, i, i, GazeEvent.None);
        Console.WriteLine($"Published sample nr: {i}, type 'exit' to exit or Enter to continue...");
        i++;
        comm.WriteGazeData(in sample);
        var line = Console.ReadLine();
        if (line == "exit")
            i = -1;
    }
}