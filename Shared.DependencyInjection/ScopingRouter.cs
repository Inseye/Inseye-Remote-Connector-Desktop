// Module name: Shared.DependencyInjection
// File name: ScopingRouter.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Routing;
using Microsoft.Extensions.Logging;
using Shared.DependencyInjection.Interfaces;
using SimpleInjector;

namespace Shared.DependencyInjection;

/// <summary>
///     Router that keeps each navigation path in separate
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ScopingRouter<T> : IScopingRouter, IDisposable, IServiceProvider
    where T : class, IRouter
{
    private readonly Stack<Scope> _stackScopes = new();

    public ScopingRouter(Container container, ILogger<ScopingRouter<T>> logger)
    {
        Container = container;
        _stackScopes.Push(new Scope(container));
        Logger = logger;
    }

    private ILogger Logger { get; }

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
        if (route == CurrentRoute)
            return;
        var poppedScope = _stackScopes.TryPop(out var oldScope);
        var currentScope = new Scope(Container);
        _stackScopes.Push(currentScope);
        try
        {
            await Router.NavigateTo(route, token);
            if (poppedScope)
                await oldScope!.DisposeAsync();
        }
        catch
        {
            await currentScope.DisposeAsync();
            _stackScopes.Pop();
            if (poppedScope)
                _stackScopes.Push(oldScope!);
            throw;
        }
    }

    public async Task NavigateToStack(Route route, CancellationToken token)
    {
        if (route == CurrentRoute)
            return;
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