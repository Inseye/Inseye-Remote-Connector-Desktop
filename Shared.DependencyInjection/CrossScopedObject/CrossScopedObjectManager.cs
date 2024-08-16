// Module name: Shared.DependencyInjection
// File name: ScopedObjectManager.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;

namespace Shared.DependencyInjection.CrossScopedObject;

internal class CrossScopedObjectManager<TService> : IDisposable, IProvider<TService>, IPublisher<TService>
    where TService : class
{
    private readonly InvokeObservable<TService?> _invokeObservable = new();
    private readonly object _lock = new();
    private int _counter;
    private TService? _instance;
    
    private TService? Instance
    {
        get => _instance;
        set
        {
            _instance = value;
            _invokeObservable.Send(value);
        }
    }

    public void Dispose()
    {
        _invokeObservable.Dispose();
        if(_instance is IDisposable disposable)
            disposable.Dispose();
    }

    internal void IncrementCounter()
    {
        lock (_lock)
        {
            _counter++;
        }
    }

    internal void DecrementCounter()
    {
        object? toDispose = null;
        lock (_lock)
        {
            if (--_counter == 0)
            {
                toDispose = _instance;
                _instance = null;
            }
        }
        DisposeManagedObject(toDispose);

    }

    public TService? Get()
    {
        return Instance;
    }

    public IObservable<TService?> ChangesStream()
    {
        return _invokeObservable;
    }

    public void Publish(TService? value)
    {
        if(EqualityComparer<TService>.Default.Equals(_instance, value))
            return;
        var old = _instance;
        lock (_lock)
        {
            if (_counter != 0)
                throw new Exception("There are scoped users that are using current instance.");
            Instance = value;
        }
        DisposeManagedObject(old);
    }

    private static void DisposeManagedObject(object? obj)
    {
        if (obj is IDisposable disposable)
            disposable.Dispose();
        else if (obj is IAsyncDisposable)
            throw new NotImplementedException("Async disposable services are not supported.");
    }
}