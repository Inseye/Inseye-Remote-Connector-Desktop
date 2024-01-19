// Module name: ViewModels
// File name: StatusViewModel.cs
// Last edit: 2024-1-31 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels;

public class StatusViewModel : ReactiveObject, IDisposable
{
    private readonly ICalibrationHandler _calibrationHandler;
    private readonly CompositeDisposable _disposable = new();
    private readonly ObservableAsPropertyHelper<EyeTrackerStatus> _eyeTrackerStatus;
    private readonly CancellationDisposable _lifeBoundedSource = new();
    private readonly IRemoteService _remoteService;
    private readonly ObservableAsPropertyHelper<RemoteServiceStatus> _remoteServiceStatus;
    private readonly ILogger<StatusViewModel> _logger;
    private readonly IRouter _router;

    public StatusViewModel(IProvider<IRemoteService> remoteService,
        ICalibrationHandler calibrationHandler, ILogger<StatusViewModel> logger, IRouter router)
    {
        _logger = logger;
        _router = router;
        _calibrationHandler = calibrationHandler;
        _remoteService = remoteService.Get()!;
        _lifeBoundedSource.DisposeWith(_disposable);
        _eyeTrackerStatus = _remoteService.EyeTrackerStatusStream
            .ToProperty(this, x => x.EyeTrackerStatus,
                () => EyeTrackerStatus.Unknown)
            .DisposeWith(_disposable);
        _remoteServiceStatus = _remoteService.ServiceStatusStream
            .ToProperty(this, x => x.RemoteServiceStatus, () => _remoteService.ServiceStatus)
            .DisposeWith(_disposable);
        _remoteService.ServiceStatusStream
            .Where(s => s == RemoteServiceStatus.Disconnected || s == RemoteServiceStatus.Disconnecting)
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(ReactiveCommand.CreateFromTask<RemoteServiceStatus, Unit>(OnServiceDisconnected)
                .DisposeWith(_disposable))
            .DisposeWith(_disposable);
        HostName = _remoteService.HostInfo.ServiceName;
        BeginCalibration = ReactiveCommand.CreateFromTask(PerformCalibration
                //, canExecute: _calibrationHandler.IsPerformingCalibration.Select(x => !x)
            )
            .DisposeWith(_disposable);
    }

    public EyeTrackerStatus EyeTrackerStatus => _eyeTrackerStatus.Value;
    public RemoteServiceStatus RemoteServiceStatus => _remoteServiceStatus.Value;
    public ReactiveCommand<Unit, Result> BeginCalibration { get; }

    public string HostName { get; }

    public void Dispose()
    {
        _logger.LogTrace($"Disposing {nameof(StatusViewModel)}");
        _disposable.Dispose();
    }

    private Task<Result> PerformCalibration()
    {
        return _calibrationHandler.CalibrationHandler(_remoteService, _lifeBoundedSource.Token);
    }

    private async Task<Unit> OnServiceDisconnected(RemoteServiceStatus status)
    {
        await _router.NavigateTo(Route.AndroidServiceSearch, default);
        return Unit.Default;
    }
}