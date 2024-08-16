// Module name: ViewModels
// File name: ICalibrationViewModel.cs
// Last edit: 2024-08-16 10:40 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive;
using EyeTrackerStreamingAvalonia.ViewModels;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels.Interfaces;

public interface ICalibrationViewModel : IViewModel
{
    public enum CalibrationStatus
    {
        None,
        InProgress,
        FinishedFailed,
        FinishedSuccessfully
    }
    
    public CalibrationStatus CalibrationState { get; }
    public string? ErrorMessage { get; }
    public ReactiveCommand<Unit, Unit> StartCalibration { get; }
    public ReactiveCommand<Unit, Unit> CancelCurrentOperation { get; }
}