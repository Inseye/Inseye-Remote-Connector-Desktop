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

public class TerminalGuiApplication : IApplication, IDisposable, IUiThreadSynchronizationContext
{
    private enum State : uint
    {
        Initializing,
        Initialized,
        Running,
        Disposed,
        Faulted
    }
    private Toplevel Top { get; }
    private ILogger<TerminalGuiApplication> Logger { get; }
    private uint _applicationStateBackingField;

    private State ApplicationState
    {
        get => (State) _applicationStateBackingField;
        set => _applicationStateBackingField = (uint) value;
    }
    private Thread MainUiThread { get; set; }
    private TaskCompletionSource WaitForRunInvoke { get; }
    private TaskCompletionSource WaitForRunFinished { get; } = new();
    private CancellationToken Token { get; set; }
    public SynchronizationContext Context { get; private set; }
    public TerminalGuiApplication(Toplevel top, ILogger<TerminalGuiApplication> logger)
    {
        Context = null!; // Context is initialized in UiThreadImplementation
        Logger = logger;
        Top = top;
        WaitForRunInvoke = new TaskCompletionSource();
        MainUiThread = new Thread(UiThreadImplementation);
        MainUiThread.Name = "MainUiThread";
        MainUiThread.Start();
        ApplicationState = State.Initializing;
        while(ApplicationState == State.Initializing) { 
            // spin wait for MainUiThread
        }

        if (ApplicationState != State.Initialized)
            throw new Exception("Failed to initialize TerminalGuiApplication. Check application logs.");
    }

    public Task Run(CancellationToken token)
    {
        if ((uint) State.Initialized != Interlocked.CompareExchange(ref _applicationStateBackingField, (uint) State.Running,
                (uint) State.Initialized))
            throw new Exception($"Application is in invalid state {ApplicationState:G}."); 
        Logger.LogInformation("Starting terminal GUI application");
        Token = token;
        try
        {
            token.ThrowIfCancellationRequested();
            WaitForRunInvoke.TrySetResult();
        }
        catch (OperationCanceledException tcs)
        {
            WaitForRunInvoke.TrySetCanceled(tcs.CancellationToken);
            throw;
        }

        return WaitForRunFinished.Task;
    }

    private void UiThreadImplementation()
    {
        // initialize application on Ui managed thread
        try
        {
            Application.Init();
            Context = SynchronizationContext.Current ?? throw new Exception("Synchronization context is null");
            ApplicationState = State.Initialized;
        }
        catch (Exception exception)
        {
            ApplicationState = State.Faulted;
            Logger.LogCritical(exception: exception, "Ui Thread Init block thrown exception");
        }
        WaitForRunInvoke.Task.Wait(); // wait for 'Run' method invoke
        // continue 
        var currScheduler = RxApp.MainThreadScheduler;
        var currTaskPoolScheduler = RxApp.TaskpoolScheduler;
        try
        {
            RxApp.MainThreadScheduler = TerminalScheduler.Default;
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            using(Token.Register(static () => Application.Invoke(() => Application.RequestStop())))
            {
                Application.Run(Top);
            }
        }
        finally
        {
            RxApp.MainThreadScheduler = currScheduler;
            RxApp.TaskpoolScheduler = currTaskPoolScheduler;
            Logger.LogInformation("Stopped terminal GUI application");
            if (Token.IsCancellationRequested)
                WaitForRunFinished.TrySetCanceled(Token);
            else
                WaitForRunFinished.TrySetResult();
        }
    }


    public void Dispose()
    {
        Logger.LogTrace(eventId: EventsId.DisposeCall, $"Disposing {nameof(TerminalGuiApplication)}");
        WaitForRunInvoke.TrySetCanceled();
        Application.Shutdown();
        MainUiThread.Join();
        ApplicationState = State.Disposed;
    }

    public void RemoveAll()
    {
        Top.RemoveAll();
    }

    public void Add(View view)
    {
        Top.Add(view);
        Top.SetNeedsDisplay();
    }
}