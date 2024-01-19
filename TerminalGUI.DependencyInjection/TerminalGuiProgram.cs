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
    public static Task Run(Action<Container> config) => Run<Window>(config);

    public static Task Run<TTop>(Action<Container> config) where TTop : Toplevel
    {
        static async Task Fun(Action<Container> config)
        {
            using var tokenSource = new CancellationTokenSource();
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
                await tokenSource.CancelAsync();
            }
        }

        return ConsoleProgram.Run(() => Fun(config));
    }
}