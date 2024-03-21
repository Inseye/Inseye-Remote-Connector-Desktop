// Module name: TerminalGUI
// File name: TerminalGuiRouter.cs
// Last edit: 2024-3-21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Reactive.Disposables;
using EyeTrackerStreaming.Shared.Extensions;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.Utility;
using EyeTrackingStreaming.ViewModels;
using ReactiveExample;
using Terminal.Gui;
using TerminalGUI.Views;

namespace TerminalGUI;

public class TerminalGuiRouter : IRouter, IDisposable
{
    private readonly InvokeObservable<bool> _canNavigateBack = new();
    private readonly Stack<Route> _routeStack = new();

    private readonly Dictionary<Route, Func<IServiceProvider, View>> _viewResolver =
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
            },
            {
                Route.ClientAuthorization,
                serviceProvider =>
                    new AuthorizationView(serviceProvider.GetServiceRequired<ClientAuthorizationViewModel>())
            }
        };

    private IDisposable? _currentView;

    public TerminalGuiRouter(TerminalGuiApplication terminalGuiApplication, IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        TerminalGuiApplication = terminalGuiApplication;
    }

    private IServiceProvider ServiceProvider { get; }
    private TerminalGuiApplication TerminalGuiApplication { get; }

    public void Dispose()
    {
        _canNavigateBack.Dispose();
        _currentView?.Dispose();
    }

    public bool CanNavigateBack => _routeStack.Count > 1;
    public IObservable<bool> CanNavigateBackObservable => _canNavigateBack;
    public Route CurrentRoute => _routeStack.Count > 0 ? _routeStack.Peek() : Route.None;

    public async Task NavigateTo(Route route, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (CurrentRoute == route)
            return;
        await TerminalGuiApplication.Context.SwitchTo();
        _currentView?.Dispose();
        _currentView = await NavigateInternal(route, token);
        _routeStack.Clear();
        _routeStack.Push(route);
    }

    public async Task NavigateToStack(Route route, CancellationToken token)
    {
        var canNavigateBackPreCall = CanNavigateBack;
        token.ThrowIfCancellationRequested();
        if (CurrentRoute == route)
            return;
        await TerminalGuiApplication.Context.SwitchTo();
        _currentView?.Dispose();
        _currentView = await NavigateInternal(route, token);
        _routeStack.Push(route);
        if (canNavigateBackPreCall != CanNavigateBack)
            _canNavigateBack.Send(CanNavigateBack);
    }

    public async Task NavigateBack(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (!CanNavigateBack)
            throw new Exception("There is not way back. (route stack has single view)");
        await TerminalGuiApplication.Context.SwitchTo();
        var canNavigateBackPreCall = CanNavigateBack;
        var currentRoute = _routeStack.Pop();
        var previousRoute = _routeStack.Peek();
        if (currentRoute == previousRoute)
            return;
        _currentView?.Dispose();
        await NavigateInternal(previousRoute, token);
        if (canNavigateBackPreCall != CanNavigateBack)
            _canNavigateBack.Send(CanNavigateBack);
    }

    private async Task<View> NavigateInternal(Route route, CancellationToken token)
    {
        var tcs = new TaskCompletionSource<View>();
        CancellationTokenRegistration registration = default;
        var disposable = TerminalScheduler.Default.Schedule(
            new
            {
                Router = this, Route = route, Task = tcs, Token = token, TerminalGuiApplication = TerminalGuiApplication
            }, static (_, state) =>
            {
                try
                {
                    state.Token.ThrowIfCancellationRequested();
                    var view = state.Router._viewResolver[state.Route].Invoke(state.Router.ServiceProvider);
                    state.TerminalGuiApplication.RemoveAll();
                    state.TerminalGuiApplication.Add(view);
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
}