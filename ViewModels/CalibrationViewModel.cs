// Module name: ViewModels
// File name: CalibrationViewModel.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Results;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels;

public class CalibrationViewModel : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly InvokeObservable<bool> _isPerformingCalibrationObservable;
    private readonly ObservableAsPropertyHelper<bool> _isPerformingCalibrationPropertyHelper;
    private readonly CancellationDisposable _lifetimeTokenSource = new();
    private string _calibrationStateDescription;
    private CancellationTokenSource? _currentCalibrationTokenSource;
    private TaskCompletionSource? _userActionTaskCompletionSource;

    public CalibrationViewModel(ILogger<CalibrationViewModel> logger)
    {
        Logger = logger;
        Logger.LogTrace(EventsId.ConstructorCall, "Constructing {type}", typeof(CalibrationViewModel));
        _calibrationStateDescription = string.Empty;
        _lifetimeTokenSource.DisposeWith(_disposables);
        var isPerformingCalibrationObservable = new InvokeObservable<bool>();
        _isPerformingCalibrationObservable = isPerformingCalibrationObservable;
        _isPerformingCalibrationPropertyHelper = isPerformingCalibrationObservable
            .Select(x => x)
            .ToProperty(this, x => x.IsPerformingCalibration)
            .DisposeWith(_disposables);
        _disposables.Add(isPerformingCalibrationObservable);
        CancelCalibrationCommand = ReactiveCommand.Create(CancelCalibration, isPerformingCalibrationObservable)
            .DisposeWith(_disposables);
        ExitCalibration = ReactiveCommand.Create(ExitCalibrationHandler);
    }

    private ILogger<CalibrationViewModel> Logger { get; }

    public string CalibrationStateDescription
    {
        get => _calibrationStateDescription;
        set
        {
            if (string.Equals(_calibrationStateDescription, value))
                return;
            ((IReactiveObject) this).RaisePropertyChanging();
            _calibrationStateDescription = value;
            ((IReactiveObject) this).RaisePropertyChanged();
        }
    }

    public bool IsPerformingCalibration
    {
        get => _isPerformingCalibrationPropertyHelper.Value;
        private set => _isPerformingCalibrationObservable.Send(value);
    }

    public ReactiveCommand<Unit, Unit> CancelCalibrationCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCalibration { get; }

    public void Dispose()
    {
        if (!_lifetimeTokenSource.IsDisposed)
        {
            Logger.LogTrace(EventsId.DisposeCall, "Disposing {type}", typeof(CalibrationViewModel));
            _disposables.Dispose();
        }
    }

    public async Task<Result> Calibrate(IRemoteService serviceUsedToPerformCalibration,
        CancellationToken token)
    {
        if (_lifetimeTokenSource.IsDisposed)
            throw new ObjectDisposedException(nameof(CalibrationViewModel));
        if (_userActionTaskCompletionSource is {Task.IsCompleted: true})
            throw new Exception("Cannot be more the one calibration in progress");
        token.ThrowIfCancellationRequested();
        _currentCalibrationTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(token, _lifetimeTokenSource.Token);
        _userActionTaskCompletionSource = new TaskCompletionSource();
        IsPerformingCalibration = true;
        Result result;
        try
        {
            try
            {
                result =
                    WrapResult(await serviceUsedToPerformCalibration.PerformCalibration(
                        _currentCalibrationTokenSource.Token));
            }
            catch (TaskCanceledException)
            {
                if (_userActionTaskCompletionSource.Task.IsCanceled)
                    return WrapResult(new ErrorResult("Cancelled by user."));
                throw; // propagate error up
            }
            catch (Exception exception)
            {
                result = WrapResult(new ErrorResult($"Exception: {exception.Message}"));
            }
            finally
            {
                IsPerformingCalibration = false;
            }

            await _userActionTaskCompletionSource.Task;
            return result;
        }
        finally
        {
            _currentCalibrationTokenSource.Dispose();
            _currentCalibrationTokenSource = null;
        }
    }

    private void CancelCalibration()
    {
        if (_lifetimeTokenSource.IsDisposed)
            throw new ObjectDisposedException(nameof(CalibrationViewModel));
        _userActionTaskCompletionSource?.TrySetCanceled();
        _currentCalibrationTokenSource?.Cancel();
    }

    private void ExitCalibrationHandler()
    {
        if (_lifetimeTokenSource.IsDisposed)
            throw new ObjectDisposedException(nameof(CalibrationViewModel));
        _userActionTaskCompletionSource?.TrySetResult();
        _currentCalibrationTokenSource?.Cancel();
    }

    private Result WrapResult(Result result)
    {
        if (result.Success)
            CalibrationStateDescription = "Calibration finished successfully";
        else if (result.Failure && result is ErrorResult error)
            CalibrationStateDescription = $"Calibration failed. {error.ErrorMessage}";
        else
            CalibrationStateDescription = "Calibration failed.";
        return result;
    }
}