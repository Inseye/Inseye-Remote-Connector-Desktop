// Module name: EyeTrackerStreamingAvalonia
// File name: CustomStyles.axml.cs
// Last edit: 2024-08-13 16:03 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace EyeTrackerStreamingAvalonia.Styling;

public class CustomStyles : Styles
{
	public CustomStyles()
	{
		AvaloniaXamlLoader.Load(this);
	}
}