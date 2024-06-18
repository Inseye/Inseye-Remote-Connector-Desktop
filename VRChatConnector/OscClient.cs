﻿// Module name: VRChatConnector
// File name: OscClient.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Net;
using System.Net.Sockets;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Pooling;
using EyeTrackerStreaming.Shared.Structs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using VRChatConnector.DataStructures;

namespace VRChatConnector;

/// <summary>
///     Osc client that sends latest gaze data to VRChat application.
/// </summary>
public class OscClient : IDisposable
{
    private readonly ObjectPool<byte[]> _smallArrayObjectPool =
        new DefaultObjectPool<byte[]>(
            new EyeTrackerStreaming.Shared.PooledObjectPolicy<byte[]>(() => new byte[128], _ => true), 10);

    private DisposeBool _disposed;

    private CancellationTokenSource? _tokenSource;

    public OscClient(ILogger<OscClient> clientLogger)
    {
        Logger = clientLogger;
        // SetEndpoint(oscClientConfiguration.Address, oscClientConfiguration.Port);
        UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _tokenSource = null;
    }

    private Socket UdpSocket { get; }
    private ILogger<OscClient> Logger { get; }

    public void Dispose()
    {
        if (!_disposed.PerformDispose())
            return;
        UdpSocket.Dispose();
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }

    /// <summary>
    ///     Schedules new gaze data sample to be sent to VRChat.
    ///     This function is thread safe.
    /// </summary>
    /// <param name="sample">Gaze data to send.</param>
    /// <param name="endpoint">Endpoint to which data is send.</param>
    public void SendGazeData(GazeDataSample sample, IPEndPoint endpoint)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var vector4 = new VrChatVector4(
            -sample.LeftEyeY * MathHelpers.RadToDeg,
            sample.LeftEyeX * MathHelpers.RadToDeg,
            -sample.RightEyeY * MathHelpers.RadToDeg,
            sample.RightEyeX * MathHelpers.RadToDeg);
        var newTcs = CancellationTokenSourcePool.Shared.Get();
        // abort current write operation because it's not containing latest gaze data
        var oldTcs = Interlocked.Exchange(ref _tokenSource, newTcs);
        if (oldTcs != null)
        {
            oldTcs.Cancel();
            oldTcs.Dispose();
        }

        var buffer = _smallArrayObjectPool.Get();
        var span = new OscDatagramBuilder(buffer).Create("/tracking/eye/LeftRightPitchYaw", vector4);
        SendAsync(endpoint, buffer, span.Length, newTcs); // fire and forget
    }

    private async void SendAsync(IPEndPoint endpoint, byte[] data, int length,
        CancellationTokenSource cancellationTokenSource)
    {
        try
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await UdpSocket.SendToAsync(new ArraySegment<byte>(data, 0, length), endpoint,
                cancellationTokenSource.Token);
            Interlocked.CompareExchange(ref _tokenSource, null, cancellationTokenSource);
        }
        catch (OperationCanceledException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception unhandledException)
        {
            Logger.LogError(unhandledException, "Unhandled exception occured while sending data to VRChat.");
        }
        finally
        {
            CancellationTokenSourcePool.Shared.Return(cancellationTokenSource);
            _smallArrayObjectPool.Return(data);
        }
    }
}