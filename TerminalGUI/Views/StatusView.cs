// Module name: TerminalGUI
// File name: StatusView.cs
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

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using EyeTrackingStreaming.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Terminal.Gui;

namespace TerminalGUI.Views;

public sealed class StatusView : DisposingView<StatusViewModel>
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
        var beginCalibrationButton = new Button
        {
            X = Pos.Percent(100.0f / 3),
            Y = Pos.Bottom(eyeTrackerStatusDisplay) + 1,
            Text = "Start calibration"
        };
        beginCalibrationButton
            .Events()
            .Clicked
            .Select(_ => Unit.Default)
            .InvokeCommand(ViewModel, x => x.BeginCalibration)
            .DisposeWith(Disposable);
        Add(beginCalibrationButton);
        var disconnectButton = new Button
        {
            X = Pos.Percent(200.0f / 3),
            Y = Pos.Bottom(eyeTrackerStatusDisplay) + 1,
            Text = "Disconnect"
        };
        disconnectButton
            .Events()
            .Clicked
            .Select(_ => Unit.Default)
            .InvokeCommand(ViewModel, x => x.Disconnect)
            .DisposeWith(Disposable);
        Add(disconnectButton);
    }
}