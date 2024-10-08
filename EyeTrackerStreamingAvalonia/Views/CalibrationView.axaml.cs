// Module name: EyeTrackerStreamingAvalonia
// File name: CalibrationView.axaml.cs
// Last edit: 2024-08-16 10:32 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Avalonia;
using Avalonia.Controls.Primitives;
using EyeTrackingStreaming.ViewModels.Interfaces;

namespace EyeTrackerStreamingAvalonia.Views;

public partial class CalibrationView : TemplatedControl
{
    private static readonly StyledProperty<ICalibrationViewModel.CalibrationStatus> CalibrationStatusProperty =
        AvaloniaProperty.Register<CalibrationView, ICalibrationViewModel.CalibrationStatus>(nameof(CalibrationStatus));

    public CalibrationView()
    {
        InitializeComponent();
    }

    private ICalibrationViewModel.CalibrationStatus CalibrationStatus
    {
        get => GetValue(CalibrationStatusProperty);
        set => SetValue(CalibrationStatusProperty, value);
    }
    
}