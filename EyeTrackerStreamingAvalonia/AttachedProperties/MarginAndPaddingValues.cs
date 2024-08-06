// Module name: EyeTrackerStreamingAvalonia
// File name: MarginAndPaddingValues.cs
// Last edit: 2024-07-30 11:16 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections.Generic;
using EyeTrackerStreamingAvalonia.Styling;

namespace EyeTrackerStreamingAvalonia.AttachedProperties;

public static class MarginAndPaddingValues
{
    static MarginAndPaddingValues()
    {
        var styles = new CustomStyles();
        var dicto = new Dictionary<string, double>();
        foreach (var resource in styles.Resources)
        {
            if (resource is {Key: string key, Value: double value})
            {
                dicto.Add(key, value);
            }
        }

        KnownMarginValues = dicto;
    }
    
    public static IReadOnlyDictionary<string, double> KnownMarginValues { get; }
}