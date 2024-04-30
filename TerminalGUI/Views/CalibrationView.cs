// Module name: TerminalGUI
// File name: CalibrationView.cs
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

internal sealed class CalibrationView : DisposingView<CalibrationViewModel>
{
    private readonly View _buttonContainer;
    private readonly Button _cancelButton;
    private readonly Button _goBackButton;

    public CalibrationView(CalibrationViewModel viewModel) : base(viewModel)
    {
        Title = "Test title";

        var textInfoBox = new Label
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(1),
            Height = Dim.Height(this) - 5,
            TextAlignment = TextAlignment.Centered,
            VerticalTextAlignment = VerticalTextAlignment.Middle
        };
        Add(textInfoBox);
        _buttonContainer = new View
        {
            X = Pos.Center(),
            Y = Pos.Bottom(textInfoBox),
            Width = Dim.Fill(1),
            Height = Dim.Fill()
        };
        Add(_buttonContainer);
        _cancelButton = new Button
        {
            Text = "Cancel calibration",
            X = Pos.Center(),
            Y = Pos.Center(),
            Height = 1
        };
        _cancelButton
            .Events()
            .Clicked
            .Select(_ => Unit.Default)
            .InvokeCommand(ViewModel, x => x.CancelCalibrationCommand)
            .DisposeWith(Disposable);
        _goBackButton = new Button
        {
            Text = "Continue",
            X = Pos.Center(),
            Y = Pos.Center()
        };
        _goBackButton
            .Events()
            .Clicked
            .Select(_ => Unit.Default)
            .InvokeCommand(ViewModel, x => x.ExitCalibration)
            .DisposeWith(Disposable);
        LoadButton(_cancelButton);
        ViewModel.WhenPropertyChanged(x => x.IsPerformingCalibration)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(prop => OnIsPerformingCalibrationChanged(prop.Value))
            .DisposeWith(Disposable);
        // calibration description
        ViewModel.WhenPropertyChanged(x => x.CalibrationStateDescription)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(x => x.Value)
            .BindTo(textInfoBox, x => x.Text)
            .DisposeWith(Disposable);
    }

    private void OnIsPerformingCalibrationChanged(bool isPerformingCalibration)
    {
        Title = isPerformingCalibration ? "Calibration in progress" : "Calibration finished.";
        LoadButton(isPerformingCalibration ? _cancelButton : _goBackButton);
    }

    private void LoadButton(Button loadedButton)
    {
        ArgumentNullException.ThrowIfNull(loadedButton, nameof(loadedButton));
        _buttonContainer.RemoveAll();
        _buttonContainer.Add(loadedButton);
    }
}