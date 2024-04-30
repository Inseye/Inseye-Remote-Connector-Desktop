// Module name: ServiceTester
// File name: GazeDataView.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive.Linq;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Shared.DependencyInjection;
using SimpleInjector;
using Terminal.Gui;

namespace ServiceTester.Views;

public sealed class GazeDataView : View, IDisposable
{
    private readonly ListCirculcarBuffer<string> _gazeDataSamples = new(50);
    private readonly ListView _listView;
    private readonly ILogger<GazeDataView> _logger;
    private readonly IProvider<IRemoteService?> _provider;
    private readonly Scope _scope;
    private IDisposable? _gazeDataSubscriber;

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
            .Retry()
            .Sample(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(HandleGazeDataSample);
    }

    private void HandleGazeDataSample(GazeDataSample sample)
    {
        var stringSample =
            $"{sample.MillisecondsUTC}, {sample.LeftEyeX}, {sample.LeftEyeY}, {sample.RightEyeX}, {sample.RightEyeY}, {sample.GazeEvent:G}";
        _gazeDataSamples.PushFront(stringSample);
        _listView.SetSource(_gazeDataSamples);
    }
}