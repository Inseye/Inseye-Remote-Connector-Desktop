// Module name: EyeTrackerStreamingAvalonia
// File name: SearchViewModel.cs
// Last edit: 2024-07-26 16:33 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Reactive;
using EyeTrackerStreaming.Shared;
using EyeTrackingStreaming.ViewModels.Interfaces;
using ReactiveUI;

namespace EyeTrackerStreamingAvalonia.ViewModels.Design;

public class SearchViewModel : ReactiveObject, ISearchViewModel
{
    public ReadOnlyObservableCollection<ServiceOffer> ServiceOffers { get; }
    public ReactiveCommand<ServiceOffer, Unit> ConnectTo { get; }
}