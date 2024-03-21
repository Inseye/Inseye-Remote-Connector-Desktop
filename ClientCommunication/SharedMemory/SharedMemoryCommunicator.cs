// Module name: ClientCommunication
// File name: SharedMemoryCommunicator.cs
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

using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using ClientCommunication.ServiceInterfaces;
using ClientCommunication.SharedMemory.Internal;
using ClientCommunication.Utility;
using EyeTrackerStreaming.Shared;
using Microsoft.Extensions.Logging;

namespace ClientCommunication.SharedMemory;

public sealed class SharedMemoryCommunicator : ISharedMemoryCommunicator
{
    private const int GazeDataBufferMaxSamples = 1_000;
    private static readonly uint GazeDataStructSize = (uint) Marshal.SizeOf<EyeTrackerDataStruct>();
    private static readonly uint SharedFileHeaderSize = (uint) Marshal.SizeOf<SharedMemoryHeader>();
    public static readonly uint TotalSize = GazeDataStructSize * GazeDataBufferMaxSamples + SharedFileHeaderSize;

    private readonly object _thisLock = new();

    // this lock protected data
    private DisposeBool _disposed = false;

    public SharedMemoryCommunicator(string sharedMemoryFilePath, ILogger<ISharedMemoryCommunicator> logger)
    {
        SharedMemoryFilePath = sharedMemoryFilePath;
        Logger = logger;
        Logger.LogTrace(EventsId.ConstructorCall,
            $"Creating {nameof(SharedMemoryCommunicator)}, {{this}}, fileName: {{sharedMemoryFileName}}", this,
            sharedMemoryFilePath);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            MmapedFile = MemoryMappedFile.CreateOrOpen(sharedMemoryFilePath,
                TotalSize, MemoryMappedFileAccess.ReadWrite);
        else
            throw new NotImplementedException(
                $"Memory mapped file was not properly implemented on current platform {RuntimeInformation.OSDescription}.");
        MappedMemoryOverlay = new MappedMemoryOverlay(MmapedFile.CreateViewAccessor(0, TotalSize));
        // prepare header
        ref var header = ref MappedMemoryOverlay.As<SharedMemoryHeader>();
        header.VersionMajor = 0;
        header.VersionMinor = 0;
        header.VersionPatch = 1;
        header.SampleSize = (uint) Marshal.SizeOf<EyeTrackerDataStruct>();
        header.HeaderSize = (uint) Marshal.SizeOf<SharedMemoryHeader>();
        header.TotalSize = (uint) TotalSize;
        header.SamplesWritten = 0;
    }

    private ILogger<ISharedMemoryCommunicator> Logger { get; }
    private MappedMemoryOverlay MappedMemoryOverlay { get; }
    private MemoryMappedFile MmapedFile { get; }

    public string SharedMemoryFilePath { get; }

    public void Dispose()
    {
        if (!_disposed.PerformDispose()) return;
        Logger.LogTrace(EventsId.DisposeCall, $"Disposing {nameof(SharedMemoryCommunicator)}, id: {{0}}", this);
        lock (_thisLock)
        {
            MmapedFile.Dispose();
        }
    }

    public void WriteGazeData(in GazeDataSample gazeData)
    {
        lock (_thisLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ref var header = ref MappedMemoryOverlay.As<SharedMemoryHeader>();
            var newSamplesValue = header.SamplesWritten + 1;
            var offset = GetGazeDataOffset((int) newSamplesValue % GazeDataBufferMaxSamples);
            ref var dataInMemory = ref MappedMemoryOverlay.As<EyeTrackerDataStruct>(offset);
            dataInMemory.Time = (ulong) gazeData.MillisecondsUTC;
            dataInMemory.LeftEyeX = gazeData.LeftEyeX;
            dataInMemory.LeftEyeY = gazeData.LeftEyeY;
            dataInMemory.RightEyeX = gazeData.RightEyeX;
            dataInMemory.RightEyeY = gazeData.RightEyeY;
            dataInMemory.EyeTrackerEvent = gazeData.GazeEvent.ToEyeTrackerEvent();
            header.SamplesWritten = newSamplesValue;
        }
    }

    private static long GetGazeDataOffset(int sampleIndex)
    {
        return SharedFileHeaderSize + sampleIndex * GazeDataStructSize;
    }
}