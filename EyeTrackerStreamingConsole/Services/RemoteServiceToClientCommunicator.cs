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
    private readonly IProvider<IGazeDataSink> _gazeDataSinkProvider;
    private readonly IProvider<IRemoteService> _remoteServiceProvider;
    private readonly ILogger<RemoteServiceToClientCommunicator> _logger;
    private readonly CompositeDisposable _disposable = new();
    private IGazeDataSink? _gazeDataSink;
    private IDisposable? _gazeStreamSubscription;
    private readonly object _objectLock = new();

    public RemoteServiceToClientCommunicator(ILogger<RemoteServiceToClientCommunicator> logger,
        IProvider<IRemoteService> remoteServiceProvider, IProvider<IGazeDataSink> gazeDataSinkProvider)
    {
        _logger = logger;
        _gazeDataSinkProvider = gazeDataSinkProvider;
        _disposable.Add((CallbackDisposable) (() => { _gazeStreamSubscription?.Dispose(); }));
        _remoteServiceProvider = remoteServiceProvider;
        remoteServiceProvider.ChangesStream().Subscribe(HandleNewRemoteService).DisposeWith(_disposable);
        gazeDataSinkProvider.ChangesStream().Subscribe(HandleNewGazeDataSink).DisposeWith(_disposable);
    }

    private void HandleNewRemoteService(IRemoteService? service)
    {
        lock (_objectLock)
        {
            if (service == null)
            {
                _logger.LogTrace("Unsubscribing from gaze data stream.");
                DisposeService();
                return;
            }

            if (_gazeDataSink != null)
                ConnectToGazeStream(service);
        }
    }

    private void HandleNewGazeDataSink(IGazeDataSink? gazeDataSink)
    {
        lock (_objectLock)
        {
            _gazeDataSink = gazeDataSink;
            if (_gazeDataSink != null && _gazeStreamSubscription == null)
            {
                if (_remoteServiceProvider.TryGet(out var service))
                    ConnectToGazeStream(service);
                else
                    _logger.LogTrace("Gaze data sink waiting for remote service with gaze data.");
            }
            else
            {
                DisposeService();
                _logger.LogTrace("Removed gaze data sink.");
            }
        }
    }

    private void ConnectToGazeStream(IRemoteService service)
    {
        _gazeStreamSubscription?.Dispose();
        _logger.LogInformation("Connecting to gaze data stream.");
        _gazeStreamSubscription = service.GazeDataStream.Subscribe(this);
    }
    
    private void HandleGazeDataAndRemoteService(IRemoteService? remoteService, IGazeDataSink? gazeDataSink)
    {
        lock (_objectLock)
        {
            switch ((remoteService, gazeDataSink))
            {
                case ({} service, {} sink):
                    break;
                case ({ } service, null):
                    break;
                case (null, {} sink):
                    break;
                case (null, null):
                    break;
            }
        }
    }

    void IDisposable.Dispose()
    {
        if (_disposed.PerformDispose())
            _disposable.Dispose();
    }

    void IObserver<GazeDataSample>.OnCompleted()
    {
        _logger.LogInformation("Current gaze data stream has finished.");
        lock (_objectLock)
        {
            DisposeService();
        }
    }

    void IObserver<GazeDataSample>.OnError(Exception error)
    {
        _logger.LogError(message: "Gaze data stream encountered error", exception: error);
        throw error;
    }

    void IObserver<GazeDataSample>.OnNext(GazeDataSample value)
    {
        _gazeDataSink?.WriteGazeData(value);
    }

    private void DisposeService()
    {
        if (_gazeStreamSubscription != null)
        {
            _logger.LogTrace("Removing gaze data subscription.");
            _gazeStreamSubscription.Dispose();
            _gazeStreamSubscription = null;
        }
    }
}