// Module name: ViewModels
// File name: CalibrationViewModel.cs
// Last edit: 2024-08-16 12:00 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using EyeTrackingStreaming.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels;

public class CalibrationViewModel : ReactiveObject, ICalibrationViewModel, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly CancellationDisposable _lifetimeTokenSource = new();
    private CancellationTokenSource? _currentCalibrationTokenSource;

    private string? _errorMessage = null;

    private ICalibrationViewModel.CalibrationStatus _state = ICalibrationViewModel.CalibrationStatus.None;

    public CalibrationViewModel(IRemoteService remoteService, ILogger<CalibrationViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(remoteService);
        ArgumentNullException.ThrowIfNull(logger);
        RemoteService = remoteService;
        Logger = logger;
        Logger.LogTrace(EventsId.ConstructorCall, "Constructing {type}", typeof(CalibrationViewModel));
        _lifetimeTokenSource.DisposeWith(_disposables);
        RemoteService.EyeTrackerStatusStream
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(status =>
            {
                if (status == EyeTrackerStatus.Calibrating)
                    IsPerformingCalibration = true;
            })
            .DisposeWith(_disposables);
        StartCalibration = ReactiveCommand.CreateFromTask(Calibrate, this.WhenPropertyChanged(x => x.IsPerformingCalibration).Select(val => !val.Value));
        CancelCurrentOperation = ReactiveCommand.Create(CancelCurrentOperationHandler);
    }

    private IRemoteService RemoteService { get; }

    private ILogger<CalibrationViewModel> Logger { get; }
    private bool _isPerformingCalibration;
    public bool IsPerformingCalibration
    {
        get => _isPerformingCalibration;
        set 
        {
            this.RaiseAndSetIfChanged(ref _isPerformingCalibration, value);
            if (value)
                CalibrationState = ICalibrationViewModel.CalibrationStatus.InProgress;
        }
    }

    public ICalibrationViewModel.CalibrationStatus CalibrationState
    {
        get => _state;
        protected set => this.RaiseAndSetIfChanged(ref _state, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        protected set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public ReactiveCommand<Unit, Unit> StartCalibration { get; }
    public ReactiveCommand<Unit, Unit> CancelCurrentOperation { get; }

    public void Dispose()
    {
        if (!_disposables.IsDisposed)
        {
            Logger.LogTrace(EventsId.DisposeCall, "Disposing {type}", typeof(CalibrationViewModel));
            _disposables.Dispose();
        }
    }

    public async Task<Unit> Calibrate()
    {
        if (_disposables.IsDisposed)
            throw new ObjectDisposedException(nameof(CalibrationViewModel));

        _currentCalibrationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeTokenSource.Token);

        IsPerformingCalibration = true;
        try
        {
            try
            {
                WrapResult(await RemoteService.PerformCalibration(
                    _currentCalibrationTokenSource.Token));
            }
            catch (Exception exception)
            {
                WrapResult(new ErrorResult($"Unhandled exception: {exception.Message}"));
            }
            finally
            {
                IsPerformingCalibration = false;
            }
        }
        finally
        {
            _currentCalibrationTokenSource.Dispose();
            _currentCalibrationTokenSource = null;
        }
        return Unit.Default;
    }

    private void CancelCurrentOperationHandler()
    {
        if (_lifetimeTokenSource.IsDisposed)
            throw new ObjectDisposedException(nameof(CalibrationViewModel));
        _currentCalibrationTokenSource?.Cancel();
        CalibrationState = ICalibrationViewModel.CalibrationStatus.None;
    }

    private void WrapResult(Result result)
    {
        if (result.Success)
        {
            CalibrationState = ICalibrationViewModel.CalibrationStatus.FinishedSuccessfully;
        }
        else if (result.Failure)
        {
            CalibrationState = ICalibrationViewModel.CalibrationStatus.FinishedFailed;
            if (result is ErrorResult error)
                ErrorMessage = error.ErrorMessage;
        }
        else
        {
            CalibrationState = ICalibrationViewModel.CalibrationStatus.FinishedFailed;
        }
    }
}