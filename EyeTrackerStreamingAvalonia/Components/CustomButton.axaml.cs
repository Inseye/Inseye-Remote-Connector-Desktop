// Module name: EyeTrackerStreamingAvalonia
// File name: CustomButton.axaml.cs
// Last edit: 2024-08-07 11:15 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.


using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EyeTrackerStreamingAvalonia.Styling;

namespace EyeTrackerStreamingAvalonia.Components;

public partial class CustomButton : Button
{
    static CustomButton()
    {
        var customStyles = new CustomStyles();
        if (customStyles.Resources.TryGetValue("--roundedSmall", out var radius) && radius is CornerRadius cornerRadius)
            CornerRadiusProperty.OverrideDefaultValue<CustomButton>(cornerRadius);
        if (customStyles.Resources.TryGetValue("--element-paddingY", out var pad1) && pad1 is double padY && customStyles.Resources.TryGetValue("--element-paddingX", out var pad2) && pad2 is double padX)
            PaddingProperty.OverrideDefaultValue<CustomButton>(new Thickness(padX, padY));
    }

    public CustomButton()
    {
        InitializeComponent();
    }
    
}