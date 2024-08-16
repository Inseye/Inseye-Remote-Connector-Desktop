// Module name: EyeTrackerStreamingAvalonia
// File name: CalibrationViewModelDesign.cs
// Last edit: 2024-08-16 10:52 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.


using EyeTrackerStreaming.Shared;
using EyeTrackingStreaming.ViewModels;
using EyeTrackingStreaming.ViewModels.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Mocks;


namespace EyeTrackerStreamingAvalonia.ViewModels.Design;

public class CalibrationViewModelDesign : CalibrationViewModel
{
    public CalibrationViewModelDesign() : base(new RemoteServiceMock(new ServiceOffer("MockName", "0.0.0.0", 1000, new Version(0, 0, 1))), NullLogger<CalibrationViewModel>.Instance)
    {
        CalibrationState = ICalibrationViewModel.CalibrationStatus.InProgress;
        ErrorMessage = "Failed calibration";
    }
}