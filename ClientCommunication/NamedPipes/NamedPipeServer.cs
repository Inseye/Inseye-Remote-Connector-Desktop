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
using ClientCommunication.SystemInterop;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace ClientCommunication.NamedPipes;

public class NamedPipeServer : IDisposable, IAsyncDisposable
{
    public const int MessageMaximumLength = 1024;
    private readonly Task _backgroundTask;

    private readonly ObjectPool<byte[]> _byteBufferObjectPool = new DefaultObjectPool<byte[]>(
        new EyeTrackerStreaming.Shared.PooledObjectPolicy<byte[]>(
            static () => new byte[MessageMaximumLength], static buffer => buffer.Length == MessageMaximumLength));

    private readonly SemaphoreSlim _connectionsSemaphore;
    private readonly CancellationDisposable _lifetimeBoundedCancellable;
    private readonly ILogger<NamedPipeServer> _logger;
    private readonly NamedPipeServerOptions _options;
    private readonly HashSet<Task> _serveClientTasks;
    private DisposeBool _disposed;

    public NamedPipeServer(IOptions<NamedPipeServerOptions> options, ILogger<NamedPipeServer> logger)
    {
        _lifetimeBoundedCancellable = new();
        _options = options.Value;
        _logger = logger;
        _connectionsSemaphore = new SemaphoreSlim(0, _options.MaximumNumberOfConcurrentClients);
        _backgroundTask = MainServerLoop(_lifetimeBoundedCancellable.Token);
        _serveClientTasks = new HashSet<Task>(_options.MaximumNumberOfConcurrentClients);
    }

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
                _logger.LogCritical(default, exception, message);
            }
        }
    }


    public void Dispose()
    {
        if (!_disposed.PerformDispose()) return;
        _lifetimeBoundedCancellable.Dispose();
        try
        {
            _connectionsSemaphore.Dispose();
            _backgroundTask.Wait();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            const string message = $"Exception occured in {nameof(NamedPipeServer)}.{nameof(MainServerLoop)}";
            _logger.LogCritical(default, exception, message);
        }
    }

    private async Task MainServerLoop(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            while (token.IsCancellationRequested)
            {
                var server = await WaitForClient(token);
                _serveClientTasks.Add(ServeClient(server, token));
            }
        }
        catch (OperationCanceledException)
        {
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

    private async Task ServeClient(NamedPipeServerStream server, CancellationToken token)
    {
        await Task.Yield();
        using var buffer = _byteBufferObjectPool.GetAutoDisposing();
        var array = buffer.Object;
        try
        {
            var requestLength = await server.ReadAsync(array, token);
            var messageType = GetMessageHeader(array.AsSpan()[..requestLength]);
            switch (messageType)
            {
                case NamedPipeMessageType.GetNamedPipeName:
                    //await server.WriteAsync(WritePipeOffer(array, ))
                    throw new NotImplementedException();
                    break;
            }
        }
        finally
        {
            _byteBufferObjectPool.Return(buffer);
            if (!_disposed)
                _connectionsSemaphore.Release();
        }
    }

    private static NamedPipeMessageType GetMessageHeader(ReadOnlySpan<byte> bytesRead)
    {
        return MemoryMarshal.Read<NamedPipeMessageType>(bytesRead);
    }

    private static ReadOnlyMemory<byte> WritePipeOffer(byte[] messageBuffer, string path)
    {
        WriteMessageType(messageBuffer, NamedPipeMessageType.NamedPipeOffer);
        if (path.Length > messageBuffer.Length)
            throw new Exception($"String to long. Maximum length is {messageBuffer.Length}");
        if (path.Any(c => c > 255))
            throw new Exception("String contains non ASCII characters.");
        throw new NotImplementedException();
        // for (var i = 0; i < path.Length; i++)
        //     messageBuffer[i + ] = (byte) path[i];
        // return messageBuffer.AsMemory(0, );
    }

    private static Memory<byte> WriteMessageType(byte[] buffer, NamedPipeMessageType type)
    {
        MemoryMarshal.Write(buffer, type);
        return new Memory<byte>(buffer, sizeof(NamedPipeMessageType), buffer.Length - sizeof(NamedPipeMessageType));
    }
}