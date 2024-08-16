// Module name: ViewModels
// File name: IStatusViewModel.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System.Net;
using System.Reactive;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreamingAvalonia.ViewModels;
using EyeTrackingStreaming.ViewModels.Modules.Interfaces;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels.Interfaces;

public interface IStatusViewModel : IViewModel
{
    /// <summary>
    ///     Current connection host name.
    /// </summary>
    public string HostName { get; }

    /// <summary>
    ///     Command that disconnects application from remote serivce.
    /// </summary>
    ReactiveCommand<Unit, Unit> Disconnect { get; }

    /// <summary>
    ///     Command that starts new eye tracker calibration procedure.
    /// </summary>
    ReactiveCommand<Unit, Unit> BeginCalibration { get; }

    /// <summary>
    ///     Current eye tracker status.
    /// </summary>
    public EyeTrackerStatus EyeTrackerStatus { get; }

    /// <summary>
    ///     Current remote service status.
    /// </summary>
    public RemoteServiceStatus RemoteServiceStatus { get; }
    /// <summary>
    ///     VrChatModuleViewModel
    /// </summary>
    public IVrChatModuleViewModel VrChatModuleViewModel { get; }
}