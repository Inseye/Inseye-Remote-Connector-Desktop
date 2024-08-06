// Module name: EyeTrackerStreamingAvalonia
// File name: SearchViewModel.cs
// Last edit: 2024-07-26 16:33 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using EyeTrackerStreaming.Shared;
using EyeTrackingStreaming.ViewModels;
using EyeTrackingStreaming.ViewModels.Interfaces;
using ReactiveUI;
using Version = EyeTrackerStreaming.Shared.Version;

namespace EyeTrackerStreamingAvalonia.ViewModels.Design;

public class SearchViewModelDesign : ReactiveObject, ISearchViewModel
{
    public ReadOnlyObservableCollection<IServiceOfferViewModel> ServiceOffers { get; } =
        new(new ObservableCollection<IServiceOfferViewModel>(new List<IServiceOfferViewModel>
        {
            new ServiceOfferViewModel(new ServiceOffer("Test service 1", "127.0.0.1", 666, new Version(1, 0, 0)), false, true),
            new ServiceOfferViewModel(new ServiceOffer("Test service 2", "192.168.1.1", 1234, new Version(0, 0, 1)), true, false) 
        }));

    public ReactiveCommand<ServiceOffer, Unit> ConnectTo { get; } = ReactiveCommand.Create<ServiceOffer, Unit>(_ => Unit.Default);
}