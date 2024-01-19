// See https://aka.ms/new-console-template for more information

using ClientCommunication.SharedMemory;
using ClientCommunication.SystemInterop;
using EyeTrackerStreaming.Shared;
// Console.WriteLine(SharedMemoryCommunicator.TotalSize);
// return;
using var comm = new SharedMemoryCommunicator();
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