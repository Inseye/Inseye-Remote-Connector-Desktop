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

public class RemoteServiceToClientCommunicator : IDisposable, IObserver<GazeDataSample>
{
    private readonly IGazeDataSink _gazeDataSink;
    private readonly ILogger<RemoteServiceToClientCommunicator> _logger;
    private readonly CompositeDisposable _disposable = new();
    private IDisposable? _gazeStreamSubscription;
    
    public RemoteServiceToClientCommunicator(ILogger<RemoteServiceToClientCommunicator> logger, IProvider<IRemoteService> remoteServiceProvider, IGazeDataSink gazeDataSink)
    {
        _logger = logger;
        _gazeDataSink = gazeDataSink;
        _disposable.Add((CallbackDisposable) (() => _gazeStreamSubscription?.Dispose()));
        remoteServiceProvider.ChangesStream().Subscribe(HandleNewRemoteService).DisposeWith(_disposable);
    }

    private void HandleNewRemoteService(IRemoteService? service)
    {
        if (service == null)
        {
            _gazeStreamSubscription?.Dispose();
            _gazeStreamSubscription = null;
            return;
        }
        _gazeStreamSubscription?.Dispose();
        _logger.LogInformation($"{nameof(RemoteServiceToClientCommunicator)} is subscribing to gaze data stream");
        _gazeStreamSubscription = service.GazeDataStream.Subscribe(this);
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }

    public void OnCompleted()
    {
        _logger.LogInformation("Current gaze data stream has finished.");
    }

    public void OnError(Exception error)
    {
        _logger.LogError(message: "Gaze data stream finished erroneously", exception: error);
        throw error;
    }

    public void OnNext(GazeDataSample value)
    {
        _gazeDataSink.WriteGazeData(value);
    }
}