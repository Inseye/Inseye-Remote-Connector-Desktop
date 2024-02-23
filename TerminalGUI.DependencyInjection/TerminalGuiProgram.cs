// Module name: TerminalGUI.DependencyInjection
// File name: TerminalGuiProgram.cs
// Last edit: 2024-2-13 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc. - All rights reserved.
// 
// All information contained herein is, and remains the property of
// Inseye Inc. The intellectual and technical concepts contained herein are
// proprietary to Inseye Inc. and may be covered by U.S. and Foreign Patents, patents
// in process, and are protected by trade secret or copyright law. Dissemination
// of this information or reproduction of this material is strictly forbidden
// unless prior written permission is obtained from Inseye Inc. Access to the source
// code contained herein is hereby forbidden to anyone except current Inseye Inc.
// employees, managers or contractors who have executed Confidentiality and
// Non-disclosure agreements explicitly covering such access.

using System.Reflection.Metadata;
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
    private sealed class PWindow : Window
    {
        public PWindow()
        {
            BorderStyle = LineStyle.None;
        }
    }

    public static Task Run(Action<Container> config, CancellationToken token = default) => Run<PWindow>(config, token);

    public static Task Run<TTop>(Action<Container> config, CancellationToken token = default) where TTop : Toplevel =>
        ConsoleProgram.Run(RunUnsafe<TTop>(config, token));

    /// <summary>
    /// Runs terminal application but doesn't handle top level exceptions in graceful way.  
    /// </summary>
    /// <param name="config">Dependency injection container configuration</param>
    /// <param name="token">GUI application cancellation token</param>
    public static Task RunUnsafe(Action<Container> config, CancellationToken token = default) =>
        RunUnsafe<PWindow>(config, token);

    /// <summary>
    /// Runs terminal application but doesn't handle top level exceptions in graceful way.  
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
}