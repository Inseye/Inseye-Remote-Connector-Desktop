// Module name: ViewModels
// File name: IServiceViewModel.cs
// Last edit: 2024-07-30 13:23 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreamingAvalonia.ViewModels;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels.Interfaces;

public interface IServiceOfferViewModel : IViewModel
{
    public ServiceOffer ServiceOffer { get; }
    public string ProtocolVersion { get; }
    public string DeviceName { get; }
    public bool IsLastItem { get; }
    public string IpAddressWithPort { get; }
    public bool IsPaired { get; }

}