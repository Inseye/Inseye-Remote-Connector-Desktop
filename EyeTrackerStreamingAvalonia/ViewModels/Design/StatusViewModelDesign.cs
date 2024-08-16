// Module name: EyeTrackerStreamingAvalonia
// File name: StatusViewModelDesign.cs
// Last edit: 2024-08-07 16:07 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Net;
using System.Reactive;
using EyeTrackerStreaming.Shared;
using EyeTrackingStreaming.ViewModels.Interfaces;
using EyeTrackingStreaming.ViewModels.Modules.Interfaces;
using ReactiveUI;

namespace EyeTrackerStreamingAvalonia.ViewModels.Design;

public class StatusViewModelDesign : ReactiveObject, IStatusViewModel
{
    public string HostName { get; } = "Quest 4 Super Ultra Pro Mega";
    public ReactiveCommand<Unit, Unit> Disconnect { get; }
    public ReactiveCommand<Unit, Unit> BeginCalibration { get; }
    public EyeTrackerStatus EyeTrackerStatus { get; }
    public RemoteServiceStatus RemoteServiceStatus { get; }
    public IVrChatModuleViewModel VrChatModuleViewModel { get; } = new VrChatModuleViewModelDesign(); 
    public bool VrChatConnectorEnabled { get; set; }
    public IPEndPoint VrChatEndpoint { get; }
}