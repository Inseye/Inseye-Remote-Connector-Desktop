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
    private static NamedPipeServerOptions DefaultOptions { get; } = new()
    {
        NamedPipeName = ServicePipeName,
        MaximumNumberOfConcurrentClients = 10
    };
    private const string ServicePipeName = "inseye.desktop-service";
    private const int MessageMaximumLength = 1024;
    private Task BackgroundTask { get; }

    private ObjectPool<byte[]> ByteBufferObjectPool { get; }= new DefaultObjectPool<byte[]>(
        new EyeTrackerStreaming.Shared.PooledObjectPolicy<byte[]>(
            static () => new byte[MessageMaximumLength], static buffer => buffer.Length == MessageMaximumLength));

    private SemaphoreSlim ConnectionsSemaphore { get; }
    private CancellationDisposable LifetimeBoundedCancellable { get; }
    private ILogger<NamedPipeServer> Logger { get; }
    private NamedPipeServerOptions Options { get; }
    private List<Task> ServerTasks { get; } = new();
    private IFactory<ISharedMemoryCommunicator, string> SharedMemoryCommunicatorFactory { get; }
    private DisposeBool _disposed;
    private object CommunicatorLock { get; } = new();
    private ISharedMemoryCommunicator? SharedMemoryCommunicator { get; set; }

    public NamedPipeServer(IFactory<ISharedMemoryCommunicator, string> communicatorFactory,
        ILogger<NamedPipeServer> logger) : this(communicatorFactory, logger, DefaultOptions) { }

    private NamedPipeServer(IFactory<ISharedMemoryCommunicator, string> communicatorFactory, ILogger<NamedPipeServer> logger, NamedPipeServerOptions options)
    {
        Options = options;
        LifetimeBoundedCancellable = new();
        Logger = logger;
        ConnectionsSemaphore = new SemaphoreSlim(Options.MaximumNumberOfConcurrentClients,
            Options.MaximumNumberOfConcurrentClients);
        BackgroundTask = MainServerLoop();
        SharedMemoryCommunicatorFactory = communicatorFactory;
    }

    public static NamedPipeServer CreateWithOptions(IFactory<ISharedMemoryCommunicator, string> communicatorFactory,
        ILogger<NamedPipeServer> logger, NamedPipeServerOptions options)
    {
        return new NamedPipeServer(communicatorFactory, logger, options);
    }

    /// <summary>
    /// Stops the server and waits for cleanup and final termination
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed.PerformDispose())
        {
            LifetimeBoundedCancellable.Dispose();
            try
            {
                await BackgroundTask;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                const string message = $"Exception occured in {nameof(NamedPipeServer)}.{nameof(MainServerLoop)}";
                Logger.LogCritical(EventsId.DisposeCall, exception, message);
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
        if (!token.CanBeCanceled || BackgroundTask.IsCompleted)
            return BackgroundTask;
        return BackgroundTask.ContinueWith(task => task, token);
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }

    private async Task MainServerLoop()
    {
        Logger.LogTrace($"Starting {nameof(NamedPipeServer)} server main loop.");
        var token = LifetimeBoundedCancellable.Token;
        token.ThrowIfCancellationRequested();
        try
        {
            var waitForClientTask = WaitForClient(token);
            lock (ServerTasks)
            {
                ServerTasks.Add(waitForClientTask);
            }

            while (!token.IsCancellationRequested)
            {
                Task<Task> whenAnyTask;
                lock (ServerTasks)
                {
                    whenAnyTask = Task.WhenAny(ServerTasks);
                }

                var finished = await whenAnyTask;
                if (finished.IsFaulted)
                    return;


                if (finished == waitForClientTask)
                {
                    Logger.LogTrace("New client has connected to named pipe server.");
                    lock (ServerTasks)
                    {
                        if (ServerTasks.Count == 1)
                            MaybeCreateSharedMemoryCommunicator();
                        ServerTasks.Add(ServeClient(waitForClientTask.Result, token));
                        waitForClientTask = WaitForClient(token);
                        ServerTasks[0] = waitForClientTask;
                    }
                }
                else
                {
                    Logger.LogTrace("Client has disconnected from named pipe server.");
                    lock (ServerTasks)
                    {
                        ServerTasks.Remove(finished);
                        if (ServerTasks.Count > 1) continue;
                        lock (CommunicatorLock)
                        {
                            SharedMemoryCommunicator?.Dispose();
                            SharedMemoryCommunicator = null;
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
            LifetimeBoundedCancellable.Dispose();
            Task whenAllTask;
            Logger.LogTrace($"Terminating {nameof(NamedPipeServer)} server main loop.");
            lock (ServerTasks)
            {
                whenAllTask = Task.WhenAll(ServerTasks);
            }

            await whenAllTask;
        }
    }

    private async Task<NamedPipeServerStream> WaitForClient(CancellationToken token)
    {
        await ConnectionsSemaphore.WaitAsync(token);
        var server = new NamedPipeServerStream(pipeName: Options.NamedPipeName,
            direction: PipeDirection.InOut, maxNumberOfServerInstances: Options.MaximumNumberOfConcurrentClients);
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
        using var bufferHandle = ByteBufferObjectPool.GetAutoDisposing();
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
                        lock (CommunicatorLock)
                        {
                            MaybeCreateSharedMemoryCommunicator();
                            message = new ServiceInfoResponseMessage
                            {
                                Version = ServiceClientCommunicationProtocol.Version,
                                SharedMemoryPath = SharedMemoryCommunicator!.SharedMemoryFilePath
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
            ByteBufferObjectPool.Return(bufferHandle);
            if (!_disposed)
                ConnectionsSemaphore.Release();
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
        if (SharedMemoryCommunicator != null)
            return;
        SharedMemoryCommunicator = SharedMemoryCommunicatorFactory.Create(GenerateSharedMemoryName(100));
    }
}