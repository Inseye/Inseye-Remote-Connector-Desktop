// Module name: EyeTrackerStreamingAvalonia
// File name: App.axaml.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EyeTrackerStreamingAvalonia.ViewModels.Abstract;
using EyeTrackerStreamingAvalonia.Views;
using Splat;

namespace EyeTrackerStreamingAvalonia;

public class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var vm = Locator.Current.GetService<IMainWindowViewModel>();
			desktop.MainWindow = new MainWindow
			{
				DataContext = vm
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
}