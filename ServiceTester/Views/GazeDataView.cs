// Module name: ServiceTester
// File name: GazeDataView.cs
// Last edit: 2024-2-19 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Reactive.Linq;
using EyeTrackerStreaming.Shared;
using Shared.DependencyInjection;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using SimpleInjector;
using Terminal.Gui;

namespace ServiceTester.Views;

public sealed class GazeDataView : View, IDisposable
{
    private readonly Scope _scope;
    private readonly IProvider<IRemoteService?> _provider;
    private readonly ListView _listView;
    private readonly ListCirculcarBuffer<string> _gazeDataSamples = new(50);
    private IDisposable? _gazeDataSubscriber;
    private readonly ILogger<GazeDataView> _logger;
    public GazeDataView(Container container, ILogger<GazeDataView> logger) // injecting container is an antypattern
    {
        _logger = logger;
        _scope = new Scope(container);
        _provider = _scope.GetInstance<IProvider<IRemoteService>>();
        _provider.ChangesStream()
            .Subscribe(HandleRemoteServiceChange) // provide implementation
            .DisposeWith(_scope);
        Add(_listView = new ListView
        {
            BorderStyle = LineStyle.Single,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Title = "GazeData preview (2 samples per second)"
        }.DisposeWith(_scope));
    }

    private void HandleRemoteServiceChange(IRemoteService? remoteService)
    {
        if (remoteService == null)
        {
            _logger.LogInformation("Unsubscribing from gaze data stream");
            _gazeDataSubscriber?.Dispose();
            return;
        }
        _logger.LogInformation("Subscribing to gaze data stream");
        _gazeDataSubscriber = remoteService.GazeDataStream
            .Sample(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(HandleGazeDataSample);
    }

    private void HandleGazeDataSample(GazeDataSample sample)
    {
        var stringSample = $"{sample.MillisecondsUTC}, {sample.LeftEyeX}, {sample.LeftEyeY}, {sample.RightEyeX}, {sample.RightEyeY}, {sample.GazeEvent:G}";
        _gazeDataSamples.PushFront(stringSample);
        _listView.SetSource(_gazeDataSamples);
    }

    void IDisposable.Dispose()
    {
        try
        {
            _scope.Dispose();
            _gazeDataSubscriber?.Dispose();
        }
        finally
        {
            base.Dispose();
        }
    }

}