// See https://aka.ms/new-console-template for more information

using ClientCommunication.SharedMemory;
using EyeTrackerStreaming.Shared;
using Microsoft.Extensions.Logging.Abstractions;

// Console.WriteLine(SharedMemoryCommunicator.TotalSize);
// return;
using var comm = new SharedMemoryCommunicator(NullLogger<SharedMemoryCommunicator>.Instance);
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