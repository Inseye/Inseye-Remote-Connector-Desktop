﻿// Module name: EyeTrackerStreamingAvalonia
// File name: Program.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.ReactiveUI;
using EyeTrackerStreaming.Shared.Configuration;
using EyeTrackerStreaming.Shared.NullObjects;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreamingAvalonia.Services;
using EyeTrackerStreamingAvalonia.ViewModels;
using EyeTrackerStreamingAvalonia.ViewModels.Interfaces;
using gRPC.DependencyInjection;
using Mocks.DependencyInjection;
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
        container.Register<IServiceProvider>(() => Lifestyle.Scoped.GetCurrentScope(container)!, Lifestyle.Scoped);
        container.UseSimpleInjectorDependencyResolver(initializer);
        // standard services
        container.RegisterGrpcApi();
        container.RegisterZeroconfServiceOfferProvider();
        container.RegisterCrossScopeManagedService<IRemoteService>(() => new NullRemoteService());
        // logging
        container.AddLogging(config =>
        {
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                
                .WriteTo.File(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "desktop_service.log"), outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Properties}{NewLine}{Exception}")
                .CreateLogger();
            config.AddSerilog(serilogLogger);
        });

        // VRChat
        container.RegisterVrChatConnector();
        container.AddOptions(new OscClientConfiguration("127.0.0.1", 9000));
        // View Models
        container.RegisterAllViewModels();
        // avalonia only view models
        container.Register<IMainWindowViewModel, MainWindowViewModel>(Lifestyle.Singleton);
        container.Register<IRouter, MainWindowViewModel>(Lifestyle.Singleton);
        container.Register<IUiThreadSynchronizationContext, AvaloniaSynchronizationContextResolver>(Lifestyle.Singleton);
        // optional mocks for development
        if (args.Contains("-m"))
            container.RegisterAllMocks();
        return BuildAvaloniaApp()
            .AfterPlatformServicesSetup(_ => container.Verify())
            .StartWithClassicDesktopLifetime(args);
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