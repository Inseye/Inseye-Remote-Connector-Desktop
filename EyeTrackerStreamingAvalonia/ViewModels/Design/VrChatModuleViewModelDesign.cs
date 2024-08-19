// Module name: EyeTrackerStreamingAvalonia
// File name: VrChatModuleViewModelDesign.cs
// Last edit: 2024-08-12 13:50 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using EyeTrackingStreaming.ViewModels.Modules.Interfaces;
using ReactiveUI;

namespace EyeTrackerStreamingAvalonia.ViewModels.Design;

public class VrChatModuleViewModelDesign : ReactiveObject, IVrChatModuleViewModel
{
	public string IpAddress => "127.0.0.1";
	public int Port => 9000;

	public bool IsEnabled { get; set; } = true;
}