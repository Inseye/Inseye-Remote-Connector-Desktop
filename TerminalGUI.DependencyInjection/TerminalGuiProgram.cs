// Module name: TerminalGUI.DependencyInjection
// File name: TerminalGuiProgram.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Shared.DependencyInjection;
using SimpleInjector;
using Terminal.Gui;
using TerminalGUI.DependencyInjection.Extensions;

namespace TerminalGUI.DependencyInjection;

public static class TerminalGuiProgram
{
    public static Task Run(Action<Container> config, CancellationToken token = default)
    {
        return Run<PWindow>(config, token);
    }

    public static Task Run<TTop>(Action<Container> config, CancellationToken token = default) where TTop : Toplevel
    {
        return ConsoleProgram.Run(RunUnsafe<TTop>(config, token));
    }

    /// <summary>
    ///     Runs terminal application but doesn't handle top level exceptions in graceful way.
    /// </summary>
    /// <param name="config">Dependency injection container configuration</param>
    /// <param name="token">GUI application cancellation token</param>
    public static Task RunUnsafe(Action<Container> config, CancellationToken token = default)
    {
        return RunUnsafe<PWindow>(config, token);
    }

    /// <summary>
    ///     Runs terminal application but doesn't handle top level exceptions in graceful way.
    /// </summary>
    /// <param name="config">Dependency injection container configuration</param>
    /// <param name="token">GUI application cancellation token</param>
    /// <typeparam name="TTop">Type of Window to display at the top.</typeparam>
    public static async Task RunUnsafe<TTop>(Action<Container> config, CancellationToken token) where TTop : Toplevel
    {
        await Task.Yield();
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        await using var terminalGuiContainer = new Container().SetDefaultOptions();
        config.Invoke(terminalGuiContainer);
        terminalGuiContainer.RegisterTerminalRouter();
        terminalGuiContainer.RegisterTerminalGuiApplication<TTop>();
        terminalGuiContainer.Verify();
        try
        {
            var guiApplication = terminalGuiContainer.GetInstance<IApplication>();
            var router = terminalGuiContainer.GetInstance<IRouter>();
            await Task.WhenAll(
                router.NavigateTo(Route.AndroidServiceSearch, tokenSource.Token),
                guiApplication.Run(tokenSource.Token));
        }
        finally
        {
            if (!tokenSource.IsCancellationRequested)
                await tokenSource.CancelAsync();
        }
    }

    private sealed class PWindow : Window
    {
        public PWindow()
        {
            BorderStyle = LineStyle.None;
        }
    }
}