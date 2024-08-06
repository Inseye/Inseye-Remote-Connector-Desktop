// Module name: EyeTrackerStreamingAvalonia
// File name: ServiceOfferView.axaml.cs
// Last edit: 2024-07-30 16:16 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;


namespace EyeTrackerStreamingAvalonia.Views;

public partial class ServiceOfferView : UserControl, ICommandSource
{
    private bool _isClickInitialized;
    public ServiceOfferView()
    {
        InitializeComponent();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isClickInitialized = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isClickInitialized)
        {
            Debug.WriteLine("Clicked");
            if(Command == null)
                return;
            if (Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        _isClickInitialized = false;
    }

    public void CanExecuteChanged(object sender, EventArgs e)
    {
        // add UI update if needed otherwise leave empty
    }
    
    
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<ServiceOfferView, ICommand?>(nameof(Command), enableDataValidation: true);
    
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<ServiceOfferView, object?>(nameof(CommandParameter));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
}