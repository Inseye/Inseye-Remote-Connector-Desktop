// Module name: ViewModels
// File name: StatusViewModel.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Configuration;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackingStreaming.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveUI;
using VRChatConnector;

namespace EyeTrackingStreaming.ViewModels;

public class StatusViewModel : ReactiveObject, IStatusViewModel, IDisposable
{
    private bool _isVrChatConnectorEnabledEnabled = false;

    private IDisposable? _oscClientSubscription;

    public StatusViewModel(IProvider<IRemoteService> remoteServiceProvider,
        ICalibrationHandler calibrationHandler, ILogger<StatusViewModel> logger, IRouter router,
        OscClient oscClient,
        IOptions<OscClientConfiguration> oscConfiguration,
        IPublisher<IRemoteService> remoteServicePublisher)
    {
        OscClient = oscClient;
        RemoteServicePublisher = remoteServicePublisher;
        Logger = logger;
        Router = router;
        CalibrationHandler = calibrationHandler;
        RemoteService = remoteServiceProvider.Get();
        LifeBoundedSource.DisposeWith(Disposable);
        EyeTrackerStatusPropertyHelper = RemoteService.EyeTrackerStatusStream
            .ToProperty(this, x => x.EyeTrackerStatus,
                () => RemoteService.EyeTrackerStatus)
            .DisposeWith(Disposable);
        RemoteServiceStatusPropertyHelper = RemoteService.ServiceStatusStream
            .ToProperty(this, x => x.RemoteServiceStatus, () => RemoteService.ServiceStatus)
            .DisposeWith(Disposable);
        RemoteService.ServiceStatusStream
            .Where(s => s == RemoteServiceStatus.Disconnected || s == RemoteServiceStatus.Disconnecting)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Finally(() => OnServiceDisconnected(RemoteServiceStatus.Disconnecting))
            .InvokeCommand(ReactiveCommand.CreateFromTask<RemoteServiceStatus, Unit>(OnServiceDisconnected)
                .DisposeWith(Disposable))
            .DisposeWith(Disposable);
        HostName = RemoteService.HostInfo.ServiceName;
        BeginCalibration = ReactiveCommand.CreateFromTask(PerformCalibration)
            .DisposeWith(Disposable);
        Disconnect = ReactiveCommand.CreateFromTask(PerformDisconnect).DisposeWith(Disposable);
        VrChatEndpoint = ConstructVrChatEndpoint(oscConfiguration.Value.Address, oscConfiguration.Value.Port);
        // SetVrChatEndpoint = ReactiveCommand.Create<(string, int), Unit>(SetVrEndpoint).DisposeWith(Disposable);
    }

    private ICalibrationHandler CalibrationHandler { get; }
    private CompositeDisposable Disposable { get; } = new();
    private ObservableAsPropertyHelper<EyeTrackerStatus> EyeTrackerStatusPropertyHelper { get; }
    private CancellationDisposable LifeBoundedSource { get; } = new();
    private IRemoteService RemoteService { get; }
    private ObservableAsPropertyHelper<RemoteServiceStatus> RemoteServiceStatusPropertyHelper { get; }
    private ILogger<StatusViewModel> Logger { get; }
    private IRouter Router { get; }
    private IPublisher<IRemoteService> RemoteServicePublisher { get; }
    private OscClient OscClient { get; }

    // public ReactiveCommand<(string address, int port), Unit> SetVrChatEndpoint { get; }

    public void Dispose()
    {
        Logger.LogTrace($"Disposing {nameof(StatusViewModel)}");
        Disposable.Dispose();
        OscClient?.Dispose();
    }

    public EyeTrackerStatus EyeTrackerStatus => EyeTrackerStatusPropertyHelper.Value;
    public RemoteServiceStatus RemoteServiceStatus => RemoteServiceStatusPropertyHelper.Value;

    public bool VrChatConnectorEnabled
    {
        get => _isVrChatConnectorEnabledEnabled;
        set => EnableVrChatConnectorInternal(value);
    }

    public IPEndPoint VrChatEndpoint { get; }
    public ReactiveCommand<Unit, Unit> BeginCalibration { get; }
    public ReactiveCommand<Unit, Unit> Disconnect { get; }

    public string HostName { get; }

    private async Task PerformCalibration()
    {
        try
        {
            await CalibrationHandler.CalibrationHandler(RemoteService, LifeBoundedSource.Token);
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception exception)
        {
            Logger.LogCritical(exception, "Failed to perform calibration.");
        }
    }

    private async Task<Unit> PerformDisconnect()
    {
        try
        {
            Logger.LogDebug("Disconnecting from remote service [user action]");
            RemoteService.Disconnect();
            RemoteServicePublisher.Publish(null);
            await Router.NavigateTo(Route.AndroidServiceSearch, default);
        }
        catch (Exception exception)
        {
            Logger.LogCritical(exception, "Failed to disconnect in response to user action");
        }

        return Unit.Default;
    }

    private async Task<Unit> OnServiceDisconnected(RemoteServiceStatus status)
    {
        try
        {
            await Router.NavigateTo(Route.AndroidServiceSearch, default);
        }
        catch (Exception exception)
        {
            Logger.LogCritical(exception, "Failed to respond to service disconnection");
        }

        return Unit.Default;
    }

    private void EnableVrChatConnectorInternal(bool isEnabled)
    {
        if (_isVrChatConnectorEnabledEnabled == isEnabled) return;
        this.RaisePropertyChanging(nameof(VrChatConnectorEnabled));
        if (isEnabled)
        {
            _oscClientSubscription =
                RemoteService.GazeDataStream.Subscribe(ForwardGazeDataSampleToOsc);
        }
        else
        {
            _oscClientSubscription?.Dispose();
            _oscClientSubscription = null;
        }

        _isVrChatConnectorEnabledEnabled = isEnabled;
        this.RaisePropertyChanged(nameof(VrChatConnectorEnabled));
    }

    // private Unit SetVrEndpoint((string address, int port) args)
    // {
    //     var (address, port) = args;
    //     var parsedAddress = IPAddress.Parse(address);
    //     if (parsedAddress.AddressFamily != AddressFamily.InterNetwork)
    //         throw new FormatException("Ip address must be an IPv4 address"); // TODO: Handle this exception
    //     if (port is < 0 or > 65535)
    //         throw new FormatException("Port must be greater then 0."); // TODO: Handle this exception
    //     if (parsedAddress.Equals(OscEndpoint.Address) && port == OscEndpoint.Port)
    //         return Unit.Default;
    //     return Unit.Default;
    // }

    private static IPEndPoint ConstructVrChatEndpoint(string address, int port)
    {
        var parsedAddress = IPAddress.Parse(address);
        if (parsedAddress.AddressFamily != AddressFamily.InterNetwork)
            throw new FormatException("Ip address must be an IPv4 address"); // TODO: Handle this exception
        if (port is < 0 or > 65535)
            throw new FormatException("Port must be greater then 0."); // TODO: Handle this exception
        return new IPEndPoint(parsedAddress, port);
    }

    private void ForwardGazeDataSampleToOsc(GazeDataSample sample)
    {
        OscClient.SendGazeData(sample, VrChatEndpoint);
    }
}