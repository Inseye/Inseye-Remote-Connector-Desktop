// Module name: TerminalGUI.Mock
// File name: Program.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Configuration;
using EyeTrackerStreaming.Shared.NullObjects;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Mocks.DependencyInjection;
using Serilog;
using Shared.DependencyInjection;
using TerminalGUI.DependencyInjection;
using ViewModels.DependencyInjection;
using VrChatConnector.DependencyInjection;

await TerminalGuiProgram.Run(container =>
{
    container.RegisterCrossScopeManagedService<IRemoteService, NullRemoteService>()
        .RegisterAllMocks()
        .RegisterAllViewModels()
        .RegisterVrChatConnector()
        .AddOptions(new OscClientConfiguration("127.0.0.1", 9000))
        .AddLogging(config =>
        {
            var serilogLogger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "desktop_service.log"))
                .CreateLogger();
            config.AddSerilog(serilogLogger);
        })
        .AddRouterLogging();
});