// Module name: ClientCommunicationTester
// File name: Program.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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