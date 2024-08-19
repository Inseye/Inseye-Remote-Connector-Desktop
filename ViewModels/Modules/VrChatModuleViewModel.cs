// Module name: ViewModels
// File name: VrChatModule.cs
// Last edit: 2024-08-14 13:44 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System.Net;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Configuration;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackingStreaming.ViewModels.Modules.Interfaces;
using Microsoft.Extensions.Options;
using ReactiveUI;
using VRChatConnector;

namespace EyeTrackingStreaming.ViewModels.Modules;

public class VrChatModuleViewModel : ReactiveObject, IVrChatModuleViewModel, IDisposable
{
	private bool _isIsEnabled;
	private IDisposable? _oscClientSubscription;

	public VrChatModuleViewModel(IRemoteService remoteService, IOptions<OscClientConfiguration> oscConfiguration,
		OscClient oscClient)
	{
		RemoteService = remoteService;
		IpAddress = oscConfiguration.Value.Address;
		Port = oscConfiguration.Value.Port;
		VrChatEndpoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
		OscClient = oscClient;
	}

	private OscClient OscClient { get; }
	private IPEndPoint VrChatEndpoint { get; }
	private IRemoteService RemoteService { get; }
	public string IpAddress { get; }
	public int Port { get; }

	public bool IsEnabled
	{
		get => _isIsEnabled;
		set => EnableVrChatConnectorInternal(value);
	}

	private void EnableVrChatConnectorInternal(bool isEnabled)
	{
		if (_isIsEnabled == isEnabled) return;
		this.RaisePropertyChanging(nameof(IsEnabled));
		if (isEnabled)
		{
			_oscClientSubscription =
				RemoteService.GazeDataStream.Subscribe(ForwardGazeDataSampleToOsc);
		}
		else
		{
			_oscClientSubscription?.Dispose();
			_oscClientSubscription = null;
		}

		_isIsEnabled = isEnabled;
		this.RaisePropertyChanged(nameof(IsEnabled));
	}

	private void ForwardGazeDataSampleToOsc(GazeDataSample sample)
	{
		OscClient.SendGazeData(sample, VrChatEndpoint);
	}

	public void Dispose()
	{
		_oscClientSubscription?.Dispose();
		OscClient.Dispose();
	}
}