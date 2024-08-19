// Module name: EyeTrackerStreamingAvalonia
// File name: TopBar.axaml.cs
// Last edit: 2024-08-07 13:20 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace EyeTrackerStreamingAvalonia.Components;

public partial class TopBar : TemplatedControl
{
    public TopBar()
    {
        InitializeComponent();
    }
    
    
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<TopBar, string>(nameof(Title), "Title");
    
    public static readonly StyledProperty<ICommand?> BackCommandProperty =
        AvaloniaProperty.Register<TopBar, ICommand?>(nameof(BackCommand), enableDataValidation: true);
    
    public static readonly StyledProperty<object?> BackCommandParameterProperty =
        AvaloniaProperty.Register<TopBar, object?>(nameof(BackCommandParameter), enableDataValidation: true);

    public static readonly StyledProperty<ICommand?> SettingsCommandProperty =
        AvaloniaProperty.Register<TopBar, ICommand?>(nameof(SettingsCommand), enableDataValidation: true);

    public static readonly StyledProperty<object?> SettingsCommandParameterProperty =
        AvaloniaProperty.Register<TopBar, object?>(nameof(SettingsCommandParameter), enableDataValidation: true);
    
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public ICommand? BackCommand
    {
        get => GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }
    
    public object? BackCommandParameter
    {
        get => GetValue(BackCommandParameterProperty);
        set => SetValue(BackCommandParameterProperty, value);
    }
    
    public ICommand? SettingsCommand
    {
        get => GetValue(SettingsCommandProperty);
        set => SetValue(SettingsCommandProperty, value);
    }
    
    public object? SettingsCommandParameter
    {
        get => GetValue(SettingsCommandParameterProperty);
        set => SetValue(SettingsCommandParameterProperty, value);
    }
    
    
        
}