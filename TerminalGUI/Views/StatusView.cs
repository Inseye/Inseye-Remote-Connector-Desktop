// Module name: TerminalGUI
// File name: StatusView.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using EyeTrackingStreaming.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Terminal.Gui;

namespace TerminalGUI.Views;

internal sealed class StatusView : DisposingView<StatusViewModel>
{
    public StatusView(StatusViewModel viewModel) : base(viewModel)
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
    }
}