// Module name: ServiceTester
// File name: Program.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.NullObjects;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using gRPC.DependencyInjection;
using Serilog;
using ServiceTester.Views;
using Shared.DependencyInjection;
using SimpleInjector;
using TerminalGUI.DependencyInjection;
using TerminalGUI.DependencyInjection.Extensions;
using ViewModels.DependencyInjection;

await TerminalGuiProgram.Run<MasterWindow>(container =>
{
    // standard services
    container.RegisterGrpcApi();
    container.RegisterZeroconfServiceOfferProvider();
    container.RegisterCrossScopeManagedService<IRemoteService, NullRemoteService>();
    container.RegisterAllViewModels();
    // custom data view
    container.Register<GazeDataView>(Lifestyle.Singleton);
    // circuit breaker
    container.AddLoggerCircuitBreaker();
    // logging
    container.AddLogging(config =>
    {
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.File(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "desktop_service.log"))
            .CreateLogger();
        config.AddSerilog(serilogLogger);
    });
});