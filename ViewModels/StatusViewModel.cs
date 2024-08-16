﻿// Module name: ViewModels
// File name: StatusViewModel.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Configuration;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreamingAvalonia.ViewModels;
using EyeTrackingStreaming.ViewModels.Interfaces;
using EyeTrackingStreaming.ViewModels.Modules.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveUI;
using VRChatConnector;

namespace EyeTrackingStreaming.ViewModels;

public class StatusViewModel : ReactiveObject, IStatusViewModel, IDisposable
{
	public StatusViewModel(IRemoteService remoteService, IVrChatModuleViewModel vrChatModuleViewModel,
		ICalibrationHandler calibrationHandler, ILogger<StatusViewModel> logger, IRouter router)
	{
		Logger = logger;
		Router = router;
		CalibrationHandler = calibrationHandler;
		RemoteService = remoteService;
		LifeBoundedSource.DisposeWith(Disposable);
		EyeTrackerStatusPropertyHelper = RemoteService.EyeTrackerStatusStream
			.ToProperty(this, x => x.EyeTrackerStatus,
				() => RemoteService.EyeTrackerStatus)
			.DisposeWith(Disposable);
		RemoteServiceStatusPropertyHelper = RemoteService.ServiceStatusStream
			.ToProperty(this, x => x.RemoteServiceStatus, () => RemoteService.ServiceStatus)
			.DisposeWith(Disposable);
		RemoteService.ServiceStatusStream
			.Where(s => s == RemoteServiceStatus.Disconnected || s == RemoteServiceStatus.Disconnecting)
			.ObserveOn(RxApp.MainThreadScheduler)
			.Finally(() => OnServiceDisconnected(RemoteServiceStatus.Disconnecting))
			.InvokeCommand(ReactiveCommand.CreateFromTask<RemoteServiceStatus, Unit>(OnServiceDisconnected)
				.DisposeWith(Disposable))
			.DisposeWith(Disposable);
		HostName = RemoteService.HostInfo.ServiceName;
		BeginCalibration = ReactiveCommand.CreateFromTask(PerformCalibration)
			.DisposeWith(Disposable);
		Disconnect = ReactiveCommand.CreateFromTask(PerformDisconnect).DisposeWith(Disposable);
		VrChatModuleViewModel = vrChatModuleViewModel;
	}

	private ICalibrationHandler CalibrationHandler { get; }
	private CompositeDisposable Disposable { get; } = new();
	private ObservableAsPropertyHelper<EyeTrackerStatus> EyeTrackerStatusPropertyHelper { get; }
	private CancellationDisposable LifeBoundedSource { get; } = new();
	private IRemoteService RemoteService { get; }
	private ObservableAsPropertyHelper<RemoteServiceStatus> RemoteServiceStatusPropertyHelper { get; }
	private ILogger<StatusViewModel> Logger { get; }
	private IRouter Router { get; }

	public void Dispose()
	{
		Logger.LogTrace($"Disposing {nameof(StatusViewModel)}");
		Disposable.Dispose();
	}

	public EyeTrackerStatus EyeTrackerStatus => EyeTrackerStatusPropertyHelper.Value;
	public RemoteServiceStatus RemoteServiceStatus => RemoteServiceStatusPropertyHelper.Value;
	public IVrChatModuleViewModel VrChatModuleViewModel { get; }

	public ReactiveCommand<Unit, Unit> BeginCalibration { get; }
	public ReactiveCommand<Unit, Unit> Disconnect { get; }

	public string HostName { get; }

	private async Task PerformCalibration()
	{
		try
		{
			await CalibrationHandler.CalibrationHandler(RemoteService, LifeBoundedSource.Token);
		}
		catch (TaskCanceledException)
		{
		}
		catch (Exception exception)
		{
			Logger.LogCritical(exception, "Failed to perform calibration.");
		}
	}

	private async Task<Unit> PerformDisconnect()
	{
		try
		{
			Logger.LogDebug("Disconnecting from remote service [user action]");
			RemoteService.Disconnect();
			await Router.NavigateTo(Route.AndroidServiceSearch, default);
		}
		catch (Exception exception)
		{
			Logger.LogCritical(exception, "Failed to disconnect in response to user action");
		}

		return Unit.Default;
	}

	private async Task<Unit> OnServiceDisconnected(RemoteServiceStatus status)
	{
		try
		{
			await Router.NavigateTo(Route.AndroidServiceSearch, default);
		}
		catch (Exception exception)
		{
			Logger.LogCritical(exception, "Failed to respond to service disconnection");
		}

		return Unit.Default;
	}
}