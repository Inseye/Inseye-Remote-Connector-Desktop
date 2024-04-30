// Module name: EyeTrackerStreamingConsole
// File name: SharedMemoryFactoryWrapper.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using ClientCommunication.ServiceInterfaces;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Structs;

namespace EyeTrackerStreamingConsole.Services;

public sealed class SharedMemoryFactoryWrapper : IFactory<ISharedMemoryCommunicator, string>
{
    public SharedMemoryFactoryWrapper(IFactory<ISharedMemoryCommunicator, string> decorated,
        IPublisher<IGazeDataSink> dataSinkPublisher)
    {
        Decorated = decorated;
        Publisher = dataSinkPublisher;
    }

    private IFactory<ISharedMemoryCommunicator, string> Decorated { get; }
    private IPublisher<IGazeDataSink> Publisher { get; }

    public ISharedMemoryCommunicator Create(string argument)
    {
        return new Wrapper(Decorated.Create(argument), Publisher);
    }

    private sealed class Wrapper : ISharedMemoryCommunicator
    {
        private DisposeBool _disposed;

        public Wrapper(ISharedMemoryCommunicator communicator, IPublisher<IGazeDataSink> gazeDataSinkPublisher)
        {
            Wrapped = communicator;
            GazeDataSinkPublisher = gazeDataSinkPublisher;
            GazeDataSinkPublisher.Publish(communicator);
        }

        private ISharedMemoryCommunicator Wrapped { get; }
        private IPublisher<IGazeDataSink> GazeDataSinkPublisher { get; }

        public void WriteGazeData(in GazeDataSample gazeDataSample)
        {
            Wrapped.WriteGazeData(gazeDataSample);
        }

        public void Dispose()
        {
            if (_disposed.PerformDispose())
                GazeDataSinkPublisher.Publish(null);
        }

        public string SharedMemoryFilePath => Wrapped.SharedMemoryFilePath;
    }
}