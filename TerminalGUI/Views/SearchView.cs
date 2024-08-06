// Module name: TerminalGUI
// File name: SearchView.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using EyeTrackingStreaming.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Terminal.Gui;

namespace TerminalGUI.Views;

internal sealed class SearchView : DisposingView<SearchViewModel>
{
	private readonly ListView _listView;

	public SearchView(SearchViewModel searchViewModel) : base(searchViewModel)
	{
		Title = "Eye tracking devices in your network";
		_listView = new ListView
		{
			X = 0,
			// Y = Pos.Bottom(updatesCounter),
			Width = Dim.Percent(100),
			Height = Dim.Fill(),
			AllowsMarking = false,
			AllowsMultipleSelection = false
		};
		ViewModel.ServiceOffers.ToObservableChangeSet()
			.Transform((offer, index) =>
				offer.ToString() ?? string.Empty // TODO: Fix me
				) 
			.Bind(DisplayedOffers)
			.ObserveOn(RxApp.MainThreadScheduler)
			.Subscribe(_ =>
			{
				var selected = _listView.SelectedItem;
				_listView.SetSource(DisplayedOffers);
				_listView.SelectedItem = Math.Min(DisplayedOffers.Count - 1, selected);
			})
			.DisposeWith(Disposable);
		ViewModel.ConnectTo.ThrownExceptions
			.ObserveOn(RxApp.MainThreadScheduler)
			.Subscribe(HandleConnectToException)
			.DisposeWith(Disposable);
		_listView
			.Events()
			.OpenSelectedItem
			.Select(args => ViewModel.ServiceOffers[args.Item])
			.InvokeCommand(ViewModel, x => x.ConnectTo)
			.DisposeWith(Disposable);
		Add(_listView);
	}

	private BindingList<string> DisplayedOffers { get; } = new();

	private void HandleConnectToException(Exception exception)
	{
		ErrorWindow.ShowError(this, exception.Message).DisposeWith(Disposable);
	}
}