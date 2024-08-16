// Module name: TerminalGUI
// File name: StatusView.cs
// Last edit: 2024-08-14 14:23 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using EyeTrackingStreaming.ViewModels.Interfaces;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace TerminalGUI.Views;

internal sealed class StatusView : DisposingView<IStatusViewModel>
{
	public StatusView(IStatusViewModel viewModel) : base(viewModel)
	{
		Title = viewModel.HostName;
		var serviceStatusLabel = new Label("Remote service status:")
		{
			X = 0,
			Y = 0,
			Height = 1
		};
		Add(serviceStatusLabel);
		var serviceStatusDisplay = new Label
		{
			X = Pos.Right(serviceStatusLabel) + 1,
			Y = serviceStatusLabel.Y,
			Height = serviceStatusLabel.Height
		};
		Add(serviceStatusDisplay);
		ViewModel
			.WhenPropertyChanged(x => x.RemoteServiceStatus)
			.Select(x => x.Value.ToString("G"))
			.ObserveOn(RxApp.MainThreadScheduler)
			.BindTo(serviceStatusDisplay, x => x.Text)
			.DisposeWith(Disposable);
		var eyeTrackerStatusLabel = new Label("EyeTracker status: ")
		{
			X = 0,
			Y = serviceStatusLabel.Y + 1,
			Height = 1
		};
		Add(eyeTrackerStatusLabel);
		var eyeTrackerStatusDisplay = new Label
		{
			X = Pos.Right(eyeTrackerStatusLabel) + 1,
			Y = eyeTrackerStatusLabel.Y,
			Height = 1
		};
		ViewModel
			.WhenPropertyChanged(x => x.EyeTrackerStatus)
			.Select(x => x.Value.ToString("G"))
			.ObserveOn(RxApp.MainThreadScheduler)
			.BindTo(eyeTrackerStatusDisplay, x => x.Text)
			.DisposeWith(Disposable);
		Add(eyeTrackerStatusDisplay);
		var buttonContainer = new View
		{
			X = Pos.Center(),
			Y = Pos.Bottom(eyeTrackerStatusDisplay) + 1,
			Width = Dim.Percent(80),
			Height = 5
		};
		Add(buttonContainer);
		var beginCalibrationButtonContainer = new View
		{
			Width = Dim.Percent(50),
			Height = Dim.Percent(100)
		};
		buttonContainer.Add(beginCalibrationButtonContainer);
		var disconnectButtonContainer = new View
		{
			X = Pos.Right(beginCalibrationButtonContainer),
			Width = Dim.Percent(50),
			Height = Dim.Percent(100)
		};
		buttonContainer.Add(disconnectButtonContainer);
		var beginCalibrationButton = new Button
		{
			X = Pos.Center(),
			Y = Pos.Center(),
			Text = "Start calibration"
		};
		beginCalibrationButton
			.Events()
			.Clicked
			.Select(_ => Unit.Default)
			.InvokeCommand(ViewModel, x => x.BeginCalibration)
			.DisposeWith(Disposable);
		beginCalibrationButtonContainer.Add(beginCalibrationButton);
		var disconnectButton = new Button
		{
			X = Pos.Center(),
			Y = Pos.Center(),
			Text = "Disconnect"
		};
		disconnectButton
			.Events()
			.Clicked
			.Select(_ => Unit.Default)
			.InvokeCommand(ViewModel, x => x.Disconnect)
			.DisposeWith(Disposable);
		disconnectButtonContainer.Add(disconnectButton);
		// VR Chat module controls
		var vrChatControlsContainer = InitializeVrChatModuleView(ViewModel, Disposable);
		vrChatControlsContainer.X = Pos.Center();
		vrChatControlsContainer.Y = Pos.Bottom(buttonContainer) + 1;
		vrChatControlsContainer.Width = Dim.Percent(100);
		vrChatControlsContainer.Height = 10;
		Add(vrChatControlsContainer);
	}

	private static View InitializeVrChatModuleView(IStatusViewModel viewModel, CompositeDisposable disposable)
	{
		var vrChatControlsContainer = new View
		{
			Title = "VRChat Module",
			BorderStyle = LineStyle.Single,
			Margin = { Thickness = new Thickness(1) }
		};
		var vrChatModuleCheckbox = new CheckBox("Enable VRChat module", viewModel.VrChatModuleViewModel.IsEnabled);
		vrChatControlsContainer.Add(vrChatModuleCheckbox);
		viewModel.VrChatModuleViewModel
			.WhenPropertyChanged(x => x.IsEnabled)
			.Select(prop => prop.Value)
			.ObserveOn(RxApp.MainThreadScheduler)
			.BindTo(vrChatModuleCheckbox, box => box.Checked)
			.DisposeWith(disposable);
		vrChatModuleCheckbox
			.Events()
			.Toggled
			.Subscribe(args =>
			{
				if (args.NewValue != null) viewModel.VrChatModuleViewModel.IsEnabled = args.NewValue.Value;
			})
			.DisposeWith(disposable);
		var vrChatAddressLabel = new Label
		{
			Text = "VRChat OSC Address",
			Width = 20,
			Y = Pos.Bottom(vrChatModuleCheckbox) + 1
		};
		vrChatControlsContainer.Add(vrChatAddressLabel);
		var vrChatAddressTextField = new TextField
		{
			Text = viewModel.VrChatModuleViewModel.IpAddress,
			Width = 15,
			X = Pos.Right(vrChatAddressLabel),
			Y = Pos.Bottom(vrChatModuleCheckbox) + 1,
			// ReadOnly = true,
			// CanFocus = false,
			ColorScheme = new ColorScheme
			{
				Focus = new Attribute(Color.White, Color.DarkGray)
			}
		};
		vrChatControlsContainer.Add(vrChatAddressTextField);
		var vrChatPortLabel = new Label
		{
			Text = "VRChat OSC Port",
			Width = 20,
			X = 0,
			Y = Pos.Bottom(vrChatAddressLabel) + 1
		};
		vrChatControlsContainer.Add(vrChatPortLabel);
		var vrChatPortTextField = new TextField
		{
			Text = viewModel.VrChatModuleViewModel.Port.ToString(),
			Width = 15,
			X = Pos.Right(vrChatPortLabel),
			Y = Pos.Bottom(vrChatAddressLabel) + 1,
			// ReadOnly = true,
			// CanFocus = false,
			ColorScheme = new ColorScheme
			{
				Focus = new Attribute(Color.White, Color.DarkGray)
			}
		};
		vrChatControlsContainer.Add(vrChatPortTextField);
		return vrChatControlsContainer;
	}
}