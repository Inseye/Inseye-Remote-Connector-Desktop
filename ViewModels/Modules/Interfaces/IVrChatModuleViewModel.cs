// Module name: ViewModels
// File name: IVrChatModuleViewModel.cs
// Last edit: 2024-08-12 13:47 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using EyeTrackerStreamingAvalonia.ViewModels;

namespace EyeTrackingStreaming.ViewModels.Modules.Interfaces;

public interface IVrChatModuleViewModel : IViewModel
{
	public string IpAddress { get; }
	public int Port { get; }
	public bool IsEnabled { get; set; }
}