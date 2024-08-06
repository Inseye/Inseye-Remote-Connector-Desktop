// Module name: EyeTrackerStreamingAvalonia
// File name: DynamicMargin.cs
// Last edit: 2024-07-29 10:13 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Avalonia;
using Avalonia.Controls;

namespace EyeTrackerStreamingAvalonia.AttachedProperties;

public static class DynamicMarginAndPadding
{
    public static readonly AttachedProperty<string> DynamicMarginProperty =
        AvaloniaProperty.RegisterAttached<Control, Control, string>("DynamicMargin");

    public static readonly AttachedProperty<string> DynamicPaddingProperty =
        AvaloniaProperty.RegisterAttached<Decorator, Decorator, string>("DynamicPadding");
    
    static DynamicMarginAndPadding()
    {
        DynamicMarginProperty.Changed.Subscribe(args =>
        {
            if (args is {Sender: Control control, NewValue.Value: { } marginString})
            {
                control.Margin = DynamicStringToThickness(control, marginString);
            }
        });
        DynamicPaddingProperty.Changed.Subscribe(args =>
        {
            if (args is {Sender: Decorator border, NewValue.Value: { } marginString})
            {
                border.Padding = DynamicStringToThickness(border, marginString);
            }
        });
    }

    public static void SetDynamicPadding(AvaloniaObject element, string value)
    {
        element.SetValue(DynamicPaddingProperty, value);
    }

    public static string GetDynamicPadding(AvaloniaObject element)
    {
        return element.GetValue(DynamicPaddingProperty);
    }
    
    public static void SetDynamicMargin(AvaloniaObject element, string value)
    {
        element.SetValue(DynamicMarginProperty, value);
    }

    public static string GetDynamicMargin(AvaloniaObject element)
    {
        return element.GetValue(DynamicMarginProperty);
    }
    
    

    private static Thickness DynamicStringToThickness(Control control, string marginString)
    {
        Span<Range> ranges = stackalloc Range[10];
        Span<double> args = stackalloc double[4];
        var spans = marginString.AsSpan().Split(ranges, ' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (spans > 4)
            throw new Exception($"To many space separated strings while parsing {marginString} for control: {control}");
        for (int i = 0; i < spans; i++)
        {
            var substring = marginString.AsSpan(ranges[i]);
            if (!(double.TryParse(substring, null, out args[i]) ||
                  MarginAndPaddingValues.KnownMarginValues.TryGetValue(substring.ToString(), out args[i]))) // TODO: Fix this allocation
                throw new Exception(
                    $"Substring must be a double parsable string or one of {nameof(MarginAndPaddingValues.KnownMarginValues)} but got: {substring} instead.");
        }

        
        if (spans == 1)
            return new Thickness(args[0]);
        if (spans == 2)
            return new Thickness(args[0], args[1]);
        if (spans == 3)
            return new Thickness(args[0], args[1], args[2], 0);
        if (spans == 4)
            return new Thickness(args[0], args[1], args[2], args[3]);
        return new Thickness();
    }
}