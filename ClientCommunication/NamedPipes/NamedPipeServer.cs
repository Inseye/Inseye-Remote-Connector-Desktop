// Module name: ClientCommunication
// File name: NamedPipeServer.cs
// Last edit: 2024-2-5 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.IO.Pipes;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using ClientCommunication.NamedPipes.Messages;
using ClientCommunication.ServiceInterfaces;
using ClientCommunication.SharedMemory;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Extensions;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace ClientCommunication.NamedPipes;

/// <summary>
/// Object that serves 
/// </summary>
public sealed class NamedPipeServer : IDisposable, IAsyncDisposable
{
    private const string ServicePipeName = "inseye.desktop-service";
    private const int MessageMaximumLength = 1024;
    private readonly Task _backgroundTask;

    private readonly ObjectPool<byte[]> _byteBufferObjectPool = new DefaultObjectPool<byte[]>(
        new EyeTrackerStreaming.Shared.PooledObjectPolicy<byte[]>(
            static () => new byte[MessageMaximumLength], static buffer => buffer.Length == MessageMaximumLength));

    private readonly SemaphoreSlim _connectionsSemaphore;
    private readonly CancellationDisposable _lifetimeBoundedCancellable;
    private readonly ILogger<NamedPipeServer> _logger;

    private readonly NamedPipeServerOptions _options = new()
    {
        NamedPipeName = ServicePipeName,
        MaximumNumberOfConcurrentClients = 10
    };

    private readonly List<Task> _serverTasks = new();
    private readonly IFactory<ISharedMemoryCommunicator, string> _sharedMemoryCommunicatorFactory;
    private DisposeBool _disposed;
    private readonly object _communicatorLock = new();
    private IDisposable? _sharedMemoryCommunicator;
    private string _sharedMemoryFilePath = String.Empty;

    public NamedPipeServer(IFactory<ISharedMemoryCommunicator, string> communicatorFactory,
        ILogger<NamedPipeServer> logger)
    {
        _lifetimeBoundedCancellable = new();
        _logger = logger;
        _connectionsSemaphore = new SemaphoreSlim(_options.MaximumNumberOfConcurrentClients,
            _options.MaximumNumberOfConcurrentClients);
        _backgroundTask = MainServerLoop();
        _sharedMemoryCommunicatorFactory = communicatorFactory;
    }

    /// <summary>
    /// Stops the server and waits for cleanup and final termination
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed.PerformDispose())
        {
            _lifetimeBoundedCancellable.Dispose();
            try
            {
                await _backgroundTask;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                const string message = $"Exception occured in {nameof(NamedPipeServer)}.{nameof(MainServerLoop)}";
                _logger.LogCritical(EventsId.DisposeCall, exception, message);
            }
        }
    }

    /// <summary>
    /// Waits for server closing the named pipe server main loop.
    /// Returns immediately if the server has finished serving clients.
    /// </summary>
    /// <param name="token">Cancellation token that breaks waiting operation.</param>
    /// <returns>Task that is finished when main loop ends</returns>
    public Task WaitForServeLoopClose(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        if (!token.CanBeCanceled || _backgroundTask.IsCompleted)
            return _backgroundTask;
        return _backgroundTask.ContinueWith(task => task, token);
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }

    private async Task MainServerLoop()
    {
        _logger.LogTrace($"Starting {nameof(NamedPipeServer)} server main loop.");
        var token = _lifetimeBoundedCancellable.Token;
        token.ThrowIfCancellationRequested();
        try
        {
            var waitForClientTask = WaitForClient(token);
            lock (_serverTasks)
            {
                _serverTasks.Add(waitForClientTask);
            }

            while (!token.IsCancellationRequested)
            {
                Task<Task> whenAnyTask;
                lock (_serverTasks)
                {
                    whenAnyTask = Task.WhenAny(_serverTasks);
                }

                var finished = await whenAnyTask;
                if (finished.IsFaulted)
                    return;


                if (finished == waitForClientTask)
                {
                    _logger.LogTrace("New client has connected to named pipe server.");
                    lock (_serverTasks)
                    {
                        if (_serverTasks.Count == 1)
                            MaybeCreateSharedMemoryCommunicator();
                        _serverTasks.Add(ServeClient(waitForClientTask.Result, token));
                        waitForClientTask = WaitForClient(token);
                        _serverTasks[0] = waitForClientTask;
                    }
                }
                else
                {
                    _logger.LogTrace("Client has disconnected from named pipe server.");
                    lock (_serverTasks)
                    {
                        _serverTasks.Remove(finished);
                        if (_serverTasks.Count > 1) continue;
                        lock (_communicatorLock)
                        {
                            _sharedMemoryCommunicator?.Dispose();
                            _sharedMemoryCommunicator = null;
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _lifetimeBoundedCancellable.Dispose();
            Task whenAllTask;
            _logger.LogTrace($"Terminating {nameof(NamedPipeServer)} server main loop.");
            lock (_serverTasks)
            {
                whenAllTask = Task.WhenAll(_serverTasks);
            }

            await whenAllTask;
        }
    }

    private async Task<NamedPipeServerStream> WaitForClient(CancellationToken token)
    {
        await _connectionsSemaphore.WaitAsync(token);
        var server = new NamedPipeServerStream(pipeName: _options.NamedPipeName,
            direction: PipeDirection.InOut, maxNumberOfServerInstances: _options.MaximumNumberOfConcurrentClients);
        await server.WaitForConnectionAsync(token);
        return server;
    }

    /// <summary>
    /// Looping task that listens to client requests and serve them
    /// </summary>
    /// <param name="server">Server used to </param>
    /// <param name="token"></param>
    private async Task ServeClient(NamedPipeServerStream server, CancellationToken token)
    {
        await Task.Yield();
        using var bufferHandle = _byteBufferObjectPool.GetAutoDisposing();
        var buffer = bufferHandle.Object;
        try
        {
            while (!token.IsCancellationRequested)
            {
                var requestLength = await server.ReadAsync(buffer, token);
                if (requestLength <= 0)
                    return;
                var messageType = GetMessageHeader(buffer.AsSpan()[..requestLength]);
                switch (messageType)
                {
                    case NamedPipeMessageType.ServiceInfoRequest:
                        ServiceInfoResponseMessage message;
                        lock (_communicatorLock)
                        {
                            MaybeCreateSharedMemoryCommunicator();
                            message = new ServiceInfoResponseMessage
                            {
                                Version = ServiceClientCommunicationProtocol.Version,
                                SharedMemoryPath = _sharedMemoryFilePath
                            };
                        }

                        var bytesOccupied = message.SerializeTo(buffer.AsSpan());
                        await server.WriteAsync(buffer, 0, bytesOccupied, token);
                        break;
                    default:
                        throw new NotImplementedException($"Failed to serve {messageType:G}");
                }
            }
        }
        finally
        {
            _byteBufferObjectPool.Return(bufferHandle);
            if (!_disposed)
                _connectionsSemaphore.Release();
        }
    }

    private static NamedPipeMessageType GetMessageHeader(ReadOnlySpan<byte> bytesRead)
    {
        return MemoryMarshal.Read<NamedPipeMessageType>(bytesRead);
    }

    private static string GenerateSharedMemoryName(int totalLength)
    {
        Random random = Random.Shared;
        using var sbHandle = SharedStringBuilderObjectPool.GetAutoDisposing();
        var sb = sbHandle.Object;
        sb.Append("Local\\");
        while (sb.Length < totalLength)
        {
            var c = (char) random.Next(33, 126);
            switch (c)
            {
                case '/':
                    continue;
                case '\\':
                    continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    private void MaybeCreateSharedMemoryCommunicator()
    {
        if (_sharedMemoryCommunicator != null)
            return;
        _sharedMemoryFilePath = GenerateSharedMemoryName(100);
        _sharedMemoryCommunicator = _sharedMemoryCommunicatorFactory.Create(_sharedMemoryFilePath);
    }
}