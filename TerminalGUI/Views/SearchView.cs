// Module name: TerminalGUI
// File name: SearchView.cs
// Last edit: 2024-1-31 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc. - All rights reserved.
// 
// All information contained herein is, and remains the property of
// Inseye Inc. The intellectual and technical concepts contained herein are
// proprietary to Inseye Inc. and may be covered by U.S. and Foreign Patents, patents
// in process, and are protected by trade secret or copyright law. Dissemination
// of this information or reproduction of this material is strictly forbidden
// unless prior written permission is obtained from Inseye Inc. Access to the source
// code contained herein is hereby forbidden to anyone except current Inseye Inc.
// employees, managers or contractors who have executed Confidentiality and
// Non-disclosure agreements explicitly covering such access.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using EyeTrackerStreaming.Shared;
using EyeTrackingStreaming.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Terminal.Gui;

namespace TerminalGUI.Views;

public sealed class SearchView : DisposingView<SearchViewModel>
{
    private readonly ListView _listView;


    public SearchView(SearchViewModel searchViewModel) : base(searchViewModel)
    {
        Title = "Eye tracking devices in your network";
        var updatesCounter = new Label
        {
            X = 0,
            Y = 0,
            Text = "Updates: 0"
        };
        ViewModel
            .WhenValueChanged(x => x.Updates)
            .Select(x => $"Updates: {x}")
            .ObserveOn(RxApp.MainThreadScheduler)
            .BindTo(updatesCounter, x => x.Text)
            .DisposeWith(Disposable);
        // Add(updatesCounter);
        _listView = new ListView
        {
            X = 0,
            // Y = Pos.Bottom(updatesCounter),
            Width = Dim.Percent(100),
            Height = Dim.Fill(),
            AllowsMarking = false,
            AllowsMultipleSelection = false
        };


        ViewModel
            .WhenPropertyChanged(x => x.ServiceOffers)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => ApplyOffers(x.Value ?? Array.Empty<ServiceOffer>()))
            .DisposeWith(Disposable);
        _listView
            .Events()
            .OpenSelectedItem
            .Select(args => ViewModel.ServiceOffers[args.Item])
            .InvokeCommand(ViewModel, x => x.ConnectTo)
            .DisposeWith(Disposable);
        Add(_listView);
    }

    private List<string> DisplayedOffers { get; set; } = new();

    private void ApplyOffers(IEnumerable<ServiceOffer> offerss)
    {
        var offers = offerss.Select((offer, index) => $"{index} {offer.ServiceName} {offer.Address}:{offer.Port} version: {offer.Version.ToString()}");
        var selected = _listView.SelectedItem;
        DisplayedOffers.Clear();
        DisplayedOffers.AddRange(offers);
        _listView.SetSource(DisplayedOffers);
        _listView.SelectedItem = Math.Min(DisplayedOffers.Count - 1, selected);
    }
}