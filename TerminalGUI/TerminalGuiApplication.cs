// Module name: TerminalGUI
// File name: TerminalGuiApplication.cs
// Last edit: 2024-1-31 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Reactive.Concurrency;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using ReactiveExample;
using ReactiveUI;
using Terminal.Gui;

namespace TerminalGUI;

public class TerminalGuiApplication : IApplication, IDisposable
{
    private readonly Toplevel _top;
    private readonly ILogger<TerminalGuiApplication> _logger;

    public TerminalGuiApplication(Toplevel top, ILogger<TerminalGuiApplication> logger)
    {
        _logger = logger;
        Application.Init();
        SynchronizationContext.SetSynchronizationContext(null);
        _top = top;
    }

    public async Task Run(CancellationToken token)
    {
        _logger.LogInformation("Starting terminal GUI application");
        var currScheduler = RxApp.MainThreadScheduler;
        var currTaskPoolScheduler = RxApp.TaskpoolScheduler;

        try
        {
            RxApp.MainThreadScheduler = TerminalScheduler.Default;
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            await using (token.Register(static () => Application.Invoke(() => Application.RequestStop())))
            {
                Application.Run(_top);
            }
        }
        finally
        {
            RxApp.MainThreadScheduler = currScheduler;
            RxApp.TaskpoolScheduler = currTaskPoolScheduler;
            _logger.LogInformation("Stopped terminal GUI application");
        }
    }


    public void Dispose()
    {
        _logger.LogTrace(eventId: EventsId.DisposeCall, $"Disposing {nameof(TerminalGuiApplication)}");
        Application.Shutdown();
    }

    public void RemoveAll()
    {
        _top.RemoveAll();
    }

    public void Add(View view)
    {
        _top.Add(view);
        _top.SetNeedsDisplay();
    }
}