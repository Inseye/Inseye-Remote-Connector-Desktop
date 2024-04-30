// Module name: TerminalGUI
// File name: TerminalGuiRouter.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive.Disposables;
using EyeTrackerStreaming.Shared.Extensions;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Structs;
using EyeTrackerStreaming.Shared.Utility;
using EyeTrackingStreaming.ViewModels;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Terminal.Gui;
using TerminalGUI.Views;

namespace TerminalGUI;

public class TerminalGuiRouter : IRouter, IDisposable
{
    private static readonly Dictionary<Route, Func<IServiceProvider, View>> ViewResolver =
        new()
        {
            {
                Route.AndroidServiceSearch,
                serviceProvider => new SearchView(serviceProvider.GetServiceRequired<SearchViewModel>())
            },
            {
                Route.ConnectionStatus,
                serviceProvider => new StatusView(serviceProvider.GetServiceRequired<StatusViewModel>())
            },
            {
                Route.Calibration,
                serviceProvider => new CalibrationView(serviceProvider.GetServiceRequired<CalibrationViewModel>())
            }
        };

    private IDisposable? _currentView;
    private DisposeBool _disposed;

    public TerminalGuiRouter(TerminalGuiApplication terminalGuiApplication, IServiceProvider serviceProvider,
        ILogger<TerminalGuiRouter> logger)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        TerminalGuiApplication = terminalGuiApplication;
        ForegroundView = new View
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            BorderStyle = LineStyle.None
            // ColorScheme = new ColorScheme(new Attribute(ColorName.Black, ColorName.Cyan))
        };
        TerminalGuiApplication.Add(ForegroundView);
    }

    private InvokeObservable<bool> CanNavigateBackInvokeObservable { get; } = new();
    private Stack<Route> RouteStack { get; } = new();
    private ILogger<TerminalGuiRouter> Logger { get; }

    private IServiceProvider ServiceProvider { get; }
    private TerminalGuiApplication TerminalGuiApplication { get; }

    /// <summary>
    ///     Container for the main view of the application.
    /// </summary>
    private View ForegroundView { get; }

    public void Dispose()
    {
        if (!_disposed.PerformDispose())
            return;
        CanNavigateBackInvokeObservable.Dispose();
        _currentView?.Dispose();
        RouteStack.Clear();
    }

    public bool CanNavigateBack => RouteStack.Count > 1;
    public IObservable<bool> CanNavigateBackObservable => CanNavigateBackInvokeObservable;
    public Route CurrentRoute => RouteStack.Count > 0 ? RouteStack.Peek() : Route.None;

    public async Task NavigateTo(Route route, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (CurrentRoute == route)
            return;
        if (_disposed)
        {
            LogRouteAborted(route);
            return;
        }

        await ((IUiThreadSynchronizationContext) TerminalGuiApplication).Context.SwitchTo();
        if (_disposed)
        {
            LogRouteAborted(route);
            return;
        }

        _currentView?.Dispose();
        _currentView = await NavigateInternal(route, token);
        RouteStack.Clear();
        RouteStack.Push(route);
    }

    public async Task NavigateToStack(Route route, CancellationToken token)
    {
        var canNavigateBackPreCall = CanNavigateBack;
        token.ThrowIfCancellationRequested();
        if (CurrentRoute == route)
            return;
        if (_disposed)
        {
            LogRouteAborted(route);
            return;
        }

        await ((IUiThreadSynchronizationContext) TerminalGuiApplication).Context.SwitchTo();
        if (_disposed)
        {
            LogRouteAborted(route);
            return;
        }

        _currentView?.Dispose();
        _currentView = await NavigateInternal(route, token);
        RouteStack.Push(route);
        if (canNavigateBackPreCall != CanNavigateBack)
            CanNavigateBackInvokeObservable.Send(CanNavigateBack);
    }

    public async Task NavigateBack(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (!CanNavigateBack)
            throw new Exception("There is not way back. (route stack has single view)");
        if (_disposed)
        {
            Logger.LogTrace("Navigation back was aborted because router was disposed.");
            return;
        }

        await ((IUiThreadSynchronizationContext) TerminalGuiApplication).Context.SwitchTo();
        if (_disposed)
        {
            Logger.LogTrace("Navigation back was aborted because router was disposed.");
            return;
        }

        var canNavigateBackPreCall = CanNavigateBack;
        var currentRoute = RouteStack.Pop();
        var previousRoute = RouteStack.Peek();
        if (currentRoute == previousRoute)
            return;
        _currentView?.Dispose();
        await NavigateInternal(previousRoute, token);
        if (_disposed)
            return;
        if (canNavigateBackPreCall != CanNavigateBack)
            CanNavigateBackInvokeObservable.Send(CanNavigateBack);
    }

    private async Task<View> NavigateInternal(Route route, CancellationToken token)
    {
        var tcs = new TaskCompletionSource<View>();
        CancellationTokenRegistration registration = default;
        var disposable = RxApp.MainThreadScheduler.Schedule(
            new
            {
                Router = this, Route = route, Task = tcs, Token = token
            }, static (_, state) =>
            {
                try
                {
                    state.Token.ThrowIfCancellationRequested();
                    var view = ViewResolver[state.Route].Invoke(state.Router.ServiceProvider);
                    state.Router.ForegroundView.RemoveAll();
                    state.Router.ForegroundView.Add(view);
                    state.Task.TrySetResult(view);
                }
                catch (Exception exc)
                {
                    state.Task.TrySetException(exc);
                    throw;
                }

                return Disposable.Create(state, static state => state.Task.TrySetCanceled(state.Token));
            });
        if (token != default)
            registration = token.Register(() =>
            {
                disposable.Dispose();
                tcs.SetCanceled(token);
            });

        try
        {
            var result = await tcs.Task;
            return result;
        }
        finally
        {
            await registration.DisposeAsync();
        }
    }

    private void LogRouteAborted(Route targetRoute)
    {
        Logger.LogTrace("Navigation to {route} was aborted because router was disposed.", targetRoute);
    }
}