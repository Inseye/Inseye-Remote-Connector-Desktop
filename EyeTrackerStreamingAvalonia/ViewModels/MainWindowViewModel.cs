// Module name: EyeTrackerStreamingAvalonia
// File name: MainWindowViewModel.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData.Binding;
using EyeTrackerStreaming.Shared.Extensions;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreamingAvalonia.ViewModels.Interfaces;
using EyeTrackingStreaming.ViewModels.Interfaces;
using ReactiveUI;
using SimpleInjector;

namespace EyeTrackerStreamingAvalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject, IMainWindowViewModel, IRouter
{
	private bool _canNavigateBack;
	private IViewModel? _currentViewModel;
	private IUiThreadSynchronizationContext UiThreadSynchronizationContext { get; }

	public MainWindowViewModel(Container masterContainer, IUiThreadSynchronizationContext uiSynchronizationContext)
	{
		MasterContainer = masterContainer;
		CurrentScope = new Scope(MasterContainer);
		CurrentRoute = Route.None;
		CurrentViewModel = GetViewModelForRoute(CurrentRoute);
		CanNavigateBackObservable = this.WhenValueChanged(obj => obj.CanNavigateBack, true, () => false);
		UiThreadSynchronizationContext = uiSynchronizationContext;
	}

	public void LoadInitialView()
	{
		CurrentViewModel = GetViewModelForRoute(Route.AndroidServiceSearch);
		CurrentRoute = Route.AndroidServiceSearch;
	}

	private Stack<Route> RoutesStack { get; } = new();
	private Scope CurrentScope { get; set; }
	private Container MasterContainer { get; }

	public IViewModel? CurrentViewModel
	{
		get => _currentViewModel;
		set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
	}

	public bool CanNavigateBack
	{
		get => _canNavigateBack;
		private set => this.RaiseAndSetIfChanged(ref _canNavigateBack, value);
	}

	public IObservable<bool> CanNavigateBackObservable { get; }

	public Route CurrentRoute { get; private set; }

	public async Task NavigateTo(Route route, CancellationToken token, object? context = null)
	{
		if (route == CurrentRoute)
			return;
		token.ThrowIfCancellationRequested();
		await UiThreadSynchronizationContext.Context;
		token.ThrowIfCancellationRequested();
		await CurrentScope.DisposeAsync();
		CurrentScope = new Scope(MasterContainer);
		CurrentViewModel = GetViewModelForRoute(route);
		CurrentRoute = route;
		CanNavigateBack = false;
	}

	public async Task NavigateToStack(Route route, CancellationToken token, object? context = null)
	{
		if (route == CurrentRoute)
			return;
		token.ThrowIfCancellationRequested();
		await UiThreadSynchronizationContext.Context;
		token.ThrowIfCancellationRequested();
		CurrentViewModel = GetViewModelForRoute(route);
		RoutesStack.Push(CurrentRoute);
		CurrentRoute = route;
		CanNavigateBack = true;
	}

	public async Task NavigateBack(CancellationToken token, object? context = null)
	{
		token.ThrowIfCancellationRequested();
		await UiThreadSynchronizationContext.Context;
		token.ThrowIfCancellationRequested();
		if (RoutesStack.Count == 0)
			throw new InvalidOperationException("There is nowhere to navigate back");
		var route = RoutesStack.Pop();
		CurrentViewModel = GetViewModelForRoute(route);
		CurrentRoute = route;
		CanNavigateBack = RoutesStack.Count != 0;
	}

	private IViewModel GetViewModelForRoute(Route route)
	{
		switch (route)
		{
			case Route.None:
				return EmptyPageViewModel.Instance;
			case Route.AndroidServiceSearch:
				return CurrentScope.GetServiceRequired<ISearchViewModel>();
			case Route.ConnectionStatus:
				return CurrentScope.GetServiceRequired<IStatusViewModel>();
			case Route.ClientAuthorization:
				return EmptyPageViewModel.Instance;
			case Route.Calibration:
				return EmptyPageViewModel.Instance;
			default:
				return EmptyPageViewModel.Instance;
		}
	}
}