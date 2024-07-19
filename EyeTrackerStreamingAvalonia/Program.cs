using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.IO;
using EyeTrackerStreaming.Shared.Configuration;
using EyeTrackerStreaming.Shared.NullObjects;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreamingAvalonia.ViewModels;
using gRPC.DependencyInjection;
using Serilog;
using Shared.DependencyInjection;
using SimpleInjector;
using Splat;
using Splat.SimpleInjector;
using ViewModels.DependencyInjection;
using VrChatConnector.DependencyInjection;

namespace EyeTrackerStreamingAvalonia;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var initializer = new SimpleInjectorInitializer();
        Locator.SetLocator(initializer);
        var container = new Container().SetDefaultOptions();
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
        
        // VRChat
        container.RegisterVrChatConnector();
        container.AddOptions(new OscClientConfiguration("127.0.0.1", 9000));
        // View Models
        container.RegisterAllViewModels();
        // avalonia router
        container.RegisterScopingRouterFor<AvaloniaRouter>(Lifestyle.Singleton);
        container.Register<MainWindowViewModel>();
        container.Verify();
        var app = BuildAvaloniaApp();
        app.StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}