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
using EyeTrackerStreamingAvalonia.Styling;

namespace EyeTrackerStreamingAvalonia.Components;

public partial class CustomButton : Button
{
    static CustomButton()
    {
        var customStyles = new CustomStyles();
        if (customStyles.TryGetResource("Rounded-Small", null, out var radius) && radius is CornerRadius cornerRadius)
            CornerRadiusProperty.OverrideDefaultValue<CustomButton>(cornerRadius);
        if (customStyles.TryGetResource("Element-PaddingY", null, out var pad1) && pad1 is double padY && customStyles.TryGetResource("Element-PaddingX", null, out var pad2) && pad2 is double padX)
            PaddingProperty.OverrideDefaultValue<CustomButton>(new Thickness(padX, padY));
    }

    public CustomButton()
    {
        InitializeComponent();
    }
    
}