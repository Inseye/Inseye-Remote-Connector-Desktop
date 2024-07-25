// Module name: EyeTrackerStreamingAvalonia
// File name: Program.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System;
using System.IO;
using Avalonia;
using Avalonia.ReactiveUI;
using EyeTrackerStreaming.Shared.Configuration;
using EyeTrackerStreaming.Shared.NullObjects;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreamingAvalonia.ViewModels;
using EyeTrackerStreamingAvalonia.ViewModels.Abstract;
using gRPC.DependencyInjection;
using Serilog;
using Shared.DependencyInjection;
using SimpleInjector;
using Splat;
using Splat.SimpleInjector;
using ViewModels.DependencyInjection;
using VrChatConnector.DependencyInjection;

namespace EyeTrackerStreamingAvalonia;

internal sealed class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static int Main(string[] args)
	{
		var initializer = new SimpleInjectorInitializer();
		Locator.SetLocator(initializer);
		var container = new Container().SetDefaultOptions();
		// container.RegisterInstance(container);
		container.Register<IServiceProvider>(() => Lifestyle.Scoped.GetCurrentScope(container)!, Lifestyle.Scoped);
		container.UseSimpleInjectorDependencyResolver(initializer);
		// standard services
		container.RegisterGrpcApi();
		container.RegisterZeroconfServiceOfferProvider();
		container.RegisterCrossScopeManagedService<IRemoteService, NullRemoteService>();
		// logging
		container.AddLogging(config =>
		{
			var serilogLogger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.Enrich.FromLogContext()
				.WriteTo.File(
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
						"desktop_service.log"))
				.CreateLogger();
			config.AddSerilog(serilogLogger);
		});
		container.Register<ILogManager, LogManager>(Lifestyle.Singleton);

		// VRChat
		container.RegisterVrChatConnector();
		container.AddOptions(new OscClientConfiguration("127.0.0.1", 9000));
		// View Models
		container.RegisterAllViewModels();
		// avalonia only view models
		container.Register<IMainWindowViewModel, MainWindowViewModel>(Lifestyle.Singleton);
		container.Register<IRouter, MainWindowViewModel>();
		var app = BuildAvaloniaApp();
		container.Verify();
		return app.StartWithClassicDesktopLifetime(args);
	}

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
	{
		return AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
	}
}