// Module name: EyeTrackerStreamingAvalonia
// File name: ServiceOfferViewModel.cs
// Last edit: 2024-07-30 16:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive;
using Avalonia.Data.Converters;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreamingAvalonia.AttachedProperties;
using EyeTrackingStreaming.ViewModels.Interfaces;
using ReactiveUI;

namespace EyeTrackerStreamingAvalonia.ViewModels.Design;

public class ServiceOfferViewModelDesign : ReactiveObject, IServiceOfferViewModel
{
    public static FuncValueConverter<bool, double> DistanceHeightConverter { get; } =
        new((isLast) =>
            isLast ? 0.0 : MarginAndPaddingValues.KnownMarginValues["--element-gap"]);

    public static FuncValueConverter<bool, string> IsPairedToSVGPathConverter { get; } =
        new(isPaired => isPaired ? "/Assets/Svg/check.svg" : "/Assets/Svg/off.svg");

    public ServiceOffer ServiceOffer { get; } =
        new("Meta Quest 4 Pro Ultra", "254.254.254.254", 60000, new Version(99, 99, 99));

    public string ProtocolVersion => ServiceOffer.Version.ToString();
    public string DeviceName => ServiceOffer.ServiceName;
    public bool IsLastItem { get; } = false;
    public string IpAddressWithPort => $"{ServiceOffer.Address}:{ServiceOffer.Port}";
    public bool IsPaired { get; } = true;
}