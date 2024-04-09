// Module name: EyeTrackerStreamingConsole
// File name: SharedMemoryFactoryWrapper.cs
// Last edit: 2024-3-4 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using ClientCommunication.ServiceInterfaces;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Structs;

namespace EyeTrackerStreamingConsole.Services;

public sealed class SharedMemoryFactoryWrapper : IFactory<ISharedMemoryCommunicator, string>
{
    private IFactory<ISharedMemoryCommunicator, string> Decorated { get; }
    private IPublisher<IGazeDataSink> Publisher { get; }
    public SharedMemoryFactoryWrapper(IFactory<ISharedMemoryCommunicator, string> decorated, IPublisher<IGazeDataSink> dataSinkPublisher)
    {
        Decorated = decorated;
        Publisher = dataSinkPublisher;
    }

    public ISharedMemoryCommunicator Create(string argument)
    {
        return new Wrapper(Decorated.Create(argument), Publisher);
    }

    private sealed class Wrapper : ISharedMemoryCommunicator
    {
        private DisposeBool _disposed;
        private ISharedMemoryCommunicator Wrapped { get; }
        private IPublisher<IGazeDataSink> GazeDataSinkPublisher { get; }
        public Wrapper(ISharedMemoryCommunicator communicator, IPublisher<IGazeDataSink> gazeDataSinkPublisher)
        {
            Wrapped = communicator;
            GazeDataSinkPublisher = gazeDataSinkPublisher;
            GazeDataSinkPublisher.Publish(communicator);
        }

        public void WriteGazeData(in GazeDataSample gazeDataSample)
        {
            Wrapped.WriteGazeData(gazeDataSample);
        }
        
        public void Dispose()
        {
            if(_disposed.PerformDispose())
                GazeDataSinkPublisher.Publish(null);
        }

        public string SharedMemoryFilePath => Wrapped.SharedMemoryFilePath;
    }
}