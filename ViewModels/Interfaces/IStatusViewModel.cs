// Module name: ViewModels
// File name: IStatusViewModel.cs
// Last edit: 2024-06-13 17:27 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.ComponentModel;
using System.Net;
using System.Reactive;
using EyeTrackerStreaming.Shared;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels.Interfaces;

public interface IStatusViewModel : INotifyPropertyChanged, INotifyPropertyChanging
{
    /// <summary>
    /// Current connection host name.
    /// </summary>
    public string HostName { get; }
    /// <summary>
    /// Command that disconnects application from remote serivce.
    /// </summary>
    ReactiveCommand<Unit, Unit> Disconnect { get; }
    /// <summary>
    /// Command that starts new eye tracker calibration procedure.
    /// </summary>
    ReactiveCommand<Unit, Unit> BeginCalibration { get; }
    /// <summary>
    /// Current eye tracker status.
    /// </summary>
    public EyeTrackerStatus EyeTrackerStatus { get; }
    /// <summary>
    /// Current remote service status.
    /// </summary>
    public RemoteServiceStatus RemoteServiceStatus { get; }
    /// <summary>
    /// VRChat module status.
    /// </summary>
    public bool VrChatConnectorEnabled { get; set; }
    /// <summary>
    /// VRChat OSC endpoint used as gaze data remote destination destination.
    /// </summary>
    public IPEndPoint VrChatEndpoint { get; }
}