// Module name: EyeTrackerStreamingConsole
// File name: RemoteServiceToClientCommunicator.cs
// Last edit: 2024-2-21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Reactive.Disposables;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;

namespace EyeTrackerStreamingConsole.Services;

/// <summary>
/// Wires together IRemoteService with IGazeData sink. 
/// </summary>
public class RemoteServiceToClientCommunicator : IDisposable, IObserver<GazeDataSample>
{
    private DisposeBool _disposed;
    private IProvider<IRemoteService> RemoteServiceProvider { get; }
    private ILogger<RemoteServiceToClientCommunicator> Logger { get; }
    private CompositeDisposable Disposable { get; } = new();
    private IGazeDataSink? GazeDataSink { get; set; }
    private IDisposable? GazeStreamSubscription { get; set; }
    private object ObjectLock { get; } = new();

    public RemoteServiceToClientCommunicator(ILogger<RemoteServiceToClientCommunicator> logger,
        IProvider<IRemoteService> remoteServiceProvider, IProvider<IGazeDataSink> gazeDataSinkProvider)
    {
        Logger = logger;
        Disposable.Add((CallbackDisposable) (() => { GazeStreamSubscription?.Dispose(); }));
        RemoteServiceProvider = remoteServiceProvider;
        remoteServiceProvider.ChangesStream().Subscribe(HandleNewRemoteService).DisposeWith(Disposable);
        gazeDataSinkProvider.ChangesStream().Subscribe(HandleNewGazeDataSink).DisposeWith(Disposable);
    }

    private void HandleNewRemoteService(IRemoteService? service)
    {
        lock (ObjectLock)
        {
            if (service == null)
            {
                Logger.LogTrace("Unsubscribing from gaze data stream.");
                DisposeService();
                return;
            }

            if (GazeDataSink != null)
                ConnectToGazeStream(service);
        }
    }

    private void HandleNewGazeDataSink(IGazeDataSink? gazeDataSink)
    {
        lock (ObjectLock)
        {
            GazeDataSink = gazeDataSink;
            if (GazeDataSink != null && GazeStreamSubscription == null)
            {
                if (RemoteServiceProvider.TryGet(out var service))
                    ConnectToGazeStream(service);
                else
                    Logger.LogTrace("Gaze data sink waiting for remote service with gaze data.");
            }
            else
            {
                DisposeService();
                Logger.LogTrace("Removed gaze data sink.");
            }
        }
    }

    private void ConnectToGazeStream(IRemoteService service)
    {
        GazeStreamSubscription?.Dispose();
        Logger.LogInformation("Connecting to gaze data stream.");
        GazeStreamSubscription = service.GazeDataStream.Subscribe(this);
    }

    void IDisposable.Dispose()
    {
        if (_disposed.PerformDispose())
            Disposable.Dispose();
    }

    void IObserver<GazeDataSample>.OnCompleted()
    {
        Logger.LogInformation("Current gaze data stream has finished.");
        lock (ObjectLock)
        {
            DisposeService();
        }
    }

    void IObserver<GazeDataSample>.OnError(Exception error)
    {
        Logger.LogError(message: "Gaze data stream encountered error", exception: error);
        throw error;
    }

    void IObserver<GazeDataSample>.OnNext(GazeDataSample value)
    {
        GazeDataSink?.WriteGazeData(value);
    }

    private void DisposeService()
    {
        if (GazeStreamSubscription == null) return;
        Logger.LogTrace("Removing gaze data subscription.");
        GazeStreamSubscription.Dispose();
        GazeStreamSubscription = null;
    }
}