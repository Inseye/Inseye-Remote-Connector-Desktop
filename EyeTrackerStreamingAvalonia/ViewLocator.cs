// Module name: EyeTrackerStreamingAvalonia
// File name: ViewLocator.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using EyeTrackerStreamingAvalonia.ViewModels;

namespace EyeTrackerStreamingAvalonia;

public class ViewLocator : IDataTemplate
{
	public Control? Build(object? data)
	{
		if (data is null)
			return null;

		var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
		var type = Type.GetType(name);

		if (type != null)
		{
			var control = (Control)Activator.CreateInstance(type)!;
			control.DataContext = data;
			return control;
		}

		return new TextBlock { Text = "Not Found: " + name };
	}

	public bool Match(object? data)
	{
		return data is IViewModel;
	}
}