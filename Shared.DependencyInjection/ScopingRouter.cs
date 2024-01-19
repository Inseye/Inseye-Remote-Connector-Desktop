// Module name: Shared.DependencyInjection
// File name: ScopingRouter.cs
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

using EyeTrackerStreaming.Shared.Routing;
using Microsoft.Extensions.Logging;
using Shared.DependencyInjection.Interfaces;
using SimpleInjector;

namespace Shared.DependencyInjection;

/// <summary>
/// Router that keeps each navigation path in separate 
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ScopingRouter<T> : IScopingRouter, IDisposable, IServiceProvider
    where T : class, IRouter
{
    private readonly Stack<Scope> _stackScopes = new();
    private ILogger Logger { get; }
    public ScopingRouter(Container container, ILogger<ScopingRouter<T>> logger)
    {
        Container = container;
        _stackScopes.Push(new Scope(container));
        Logger = logger;
    }

    private IRouter Router => _stackScopes.Peek().GetInstance<T>();
    private Container Container { get; }

    public void Dispose()
    {
        while (_stackScopes.Count > 0)
        {
            var scope = _stackScopes.Pop();
            scope.Dispose();
        }
    }

    public bool CanNavigateBack => Router.CanNavigateBack;
    public IObservable<bool> CanNavigateBackObservable => Router.CanNavigateBackObservable;
    public Route CurrentRoute => Router.CurrentRoute;
    public Scope CurrentRouteScope => _stackScopes.Peek();

    public async Task NavigateTo(Route route, CancellationToken token)
    {
        var oldScope = _stackScopes.Pop();
        var currentScope = new Scope(Container);
        _stackScopes.Push(currentScope);
        try
        {
            await Router.NavigateTo(route, token);
            await oldScope.DisposeAsync();
        }
        catch
        {
            await currentScope.DisposeAsync();
            _stackScopes.Pop();
            _stackScopes.Push(oldScope);
            throw;
        }
    }

    public async Task NavigateToStack(Route route, CancellationToken token)
    {
        var currentScope = new Scope(Container);
        _stackScopes.Push(currentScope);
        try
        {
            await Router.NavigateToStack(route, token);
        }
        catch
        {
            await currentScope.DisposeAsync();
            _stackScopes.Pop();
            throw;
        }
    }

    public async Task NavigateBack(CancellationToken token)
    {
        if (!CanNavigateBack)
            throw new Exception("Can't navigate back");
        var currentScope = _stackScopes.Pop();
        await currentScope.DisposeAsync();
        await Router.NavigateBack(token);
        await currentScope.DisposeAsync();
    }

    public object? GetService(Type serviceType)
    {
        return ((IServiceProvider) _stackScopes.Peek()).GetService(serviceType);
    }
}