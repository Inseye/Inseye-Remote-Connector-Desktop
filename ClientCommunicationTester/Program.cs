// See https://aka.ms/new-console-template for more information

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
    using var comm = new SharedMemoryCommunicator("Local\\Inseye-Remote-Connector-Shared-Memory", NullLogger<SharedMemoryCommunicator>.Instance);
    GazeDataSample sample;
    int i = 1;
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