// Module name: ClientCommunication
// File name: NamedPipeServer.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.IO.Pipes;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using ClientCommunication.NamedPipes.Messages;
using ClientCommunication.ServiceInterfaces;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Extensions;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Structs;
using EyeTrackerStreaming.Shared.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace ClientCommunication.NamedPipes;

public sealed class NamedPipeServer : IDisposable, IAsyncDisposable
{
    private const string ServicePipeName = "inseye.desktop-service";
    private const int MessageMaximumLength = 1024;
    private DisposeBool _disposed;

    public NamedPipeServer(IFactory<ISharedMemoryCommunicator, string> communicatorFactory,
        ILogger<NamedPipeServer> logger) : this(communicatorFactory, logger,
        DefaultOptions, null!)
    {
    }

    private NamedPipeServer(IFactory<ISharedMemoryCommunicator, string> communicatorFactory,
        ILogger<NamedPipeServer> logger, NamedPipeServerOptions options, IClientAuthorization authorization)
    {
        ClientAuthorization = authorization;
        Options = options;
        LifetimeBoundedCancellable = new CancellationDisposable();
        Logger = logger;
        ConnectionsSemaphore = new SemaphoreSlim(Options.MaximumNumberOfConcurrentClients,
            Options.MaximumNumberOfConcurrentClients);
        BackgroundTask = MainServerLoop();
        SharedMemoryCommunicatorManager =
            new SynchronizedSharedObjectManager<ISharedMemoryCommunicator>(new Factory(communicatorFactory));
    }

    private static NamedPipeServerOptions DefaultOptions { get; } = new()
    {
        NamedPipeName = ServicePipeName,
        MaximumNumberOfConcurrentClients = 10
    };

    private Task BackgroundTask { get; }

    private ObjectPool<byte[]> ByteBufferObjectPool { get; } = new DefaultObjectPool<byte[]>(
        new EyeTrackerStreaming.Shared.PooledObjectPolicy<byte[]>(
            static () => new byte[MessageMaximumLength], static buffer => buffer.Length == MessageMaximumLength));

    private SemaphoreSlim ConnectionsSemaphore { get; }
    private CancellationDisposable LifetimeBoundedCancellable { get; }
    private ILogger<NamedPipeServer> Logger { get; }
    private NamedPipeServerOptions Options { get; }
    private List<Task> ServerTasks { get; } = new();
    private SynchronizedSharedObjectManager<ISharedMemoryCommunicator> SharedMemoryCommunicatorManager { get; }
    private IClientAuthorization ClientAuthorization { get; }

    /// <summary>
    ///     Stops the server and waits for cleanup and final termination
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

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }

    public static NamedPipeServer CreateWithOptions(IFactory<ISharedMemoryCommunicator, string> communicatorFactory,
        ILogger<NamedPipeServer> logger, NamedPipeServerOptions options, IClientAuthorization authorization)
    {
        return new NamedPipeServer(communicatorFactory, logger, options, authorization);
    }

    /// <summary>
    ///     Waits for server closing the named pipe server main loop.
    ///     Returns immediately if the server has finished serving clients.
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
                    var result = waitForClientTask.Result;
                    Logger.LogTrace("New client has connected to named pipe server.");
                    lock (ServerTasks)
                    {
                        ServerTasks.Add(ServeClient(result, token));
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
        var server = new NamedPipeServerStream(Options.NamedPipeName,
            PipeDirection.InOut, Options.MaximumNumberOfConcurrentClients);
        await server.WaitForConnectionAsync(token);
        return server;
    }

    /// <summary>
    ///     Looping task that listens to client requests and serve them
    /// </summary>
    /// <param name="server">Server used to </param>
    /// <param name="token"></param>
    private async Task ServeClient(NamedPipeServerStream server, CancellationToken token)
    {
        await Task.Yield();
        using var bufferHandle = ByteBufferObjectPool.GetAutoDisposing();
        var buffer = bufferHandle.Object;
        SynchronizedSharedObjectManager<ISharedMemoryCommunicator>.SynchronizedSharedObjectToken? objectToken = null;
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
                        objectToken ??= SharedMemoryCommunicatorManager.Get();
                        message = new ServiceInfoResponseMessage
                        {
                            Version = ServiceClientCommunicationProtocol.Version,
                            SharedMemoryPath = objectToken.Object.SharedMemoryFilePath
                        };


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
            objectToken?.Dispose();
            ByteBufferObjectPool.Return(bufferHandle);
            if (!_disposed)
                ConnectionsSemaphore.Release();
        }
    }

    private static NamedPipeMessageType GetMessageHeader(ReadOnlySpan<byte> bytesRead)
    {
        return MemoryMarshal.Read<NamedPipeMessageType>(bytesRead);
    }

    private class Factory : IFactory<ISharedMemoryCommunicator>
    {
        public Factory(IFactory<ISharedMemoryCommunicator, string> wrapped)
        {
            WrappedFactory = wrapped;
        }

        private IFactory<ISharedMemoryCommunicator, string> WrappedFactory { get; }

        public ISharedMemoryCommunicator Create()
        {
            return WrappedFactory.Create(GenerateSharedMemoryName(100));
        }

        private static string GenerateSharedMemoryName(int totalLength)
        {
            var random = Random.Shared;
            using var sbHandle = StringBuilderPool.Shared.GetAutoDisposing();
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
    }
}