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

namespace EyeTrackerStreamingConsole.Services;

public sealed class SharedMemoryFactoryWrapper : IFactory<ISharedMemoryCommunicator, string>
{
    private readonly IFactory<ISharedMemoryCommunicator, string> _decorated;
    private readonly IPublisher<IGazeDataSink> _publisher;
    public SharedMemoryFactoryWrapper(IFactory<ISharedMemoryCommunicator, string> decorated, IPublisher<IGazeDataSink> dataSinkPublisher)
    {
        _decorated = decorated;
        _publisher = dataSinkPublisher;
    }

    public ISharedMemoryCommunicator Create(string argument)
    {
        return new Wrapper(_decorated.Create(argument), _publisher);
    }

    private sealed class Wrapper : ISharedMemoryCommunicator
    {
        private DisposeBool _disposed;
        private readonly ISharedMemoryCommunicator _wrapped;
        private readonly IPublisher<IGazeDataSink> _gazeDataSinkPublisher;
        public Wrapper(ISharedMemoryCommunicator communicator, IPublisher<IGazeDataSink> gazeDataSinkPublisher)
        {
            _wrapped = communicator;
            _gazeDataSinkPublisher = gazeDataSinkPublisher;
            _gazeDataSinkPublisher.Publish(communicator);
        }

        public void WriteGazeData(in GazeDataSample gazeDataSample)
        {
            _wrapped.WriteGazeData(gazeDataSample);
        }
        
        public void Dispose()
        {
            if(_disposed.PerformDispose())
                _gazeDataSinkPublisher.Publish(null);
        }
    }
}