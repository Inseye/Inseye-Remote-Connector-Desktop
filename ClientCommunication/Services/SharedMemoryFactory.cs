// Module name: ClientCommunication
// File name: SharedMemoryFactory.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using ClientCommunication.ServiceInterfaces;
using ClientCommunication.SharedMemory;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;

namespace ClientCommunication.Utility;

public class SharedMemoryFactory : IFactory<ISharedMemoryCommunicator, string>
{
    private readonly ILogger<ISharedMemoryCommunicator> _logger;

    public SharedMemoryFactory(ILogger<ISharedMemoryCommunicator> logger)
    {
        _logger = logger;
    }

    public ISharedMemoryCommunicator Create(string sharedMemoryFileName)
    {
        return new SharedMemoryCommunicator(sharedMemoryFileName, _logger);
    }
}