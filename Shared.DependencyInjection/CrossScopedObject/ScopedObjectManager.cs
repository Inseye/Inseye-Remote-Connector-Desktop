// Module name: Shared.DependencyInjection
// File name: ScopedObjectManager.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Utility;

namespace Shared.DependencyInjection.CrossScopedObject;

internal class ScopedObjectManager<TInterface, TConcrete> : IDisposable
    where TConcrete : TInterface
{
    private readonly InvokeObservable<TInterface?> _invokeObservable = new();
    private readonly object _lock = new();
    private int _counter;
    private TConcrete? _instance;

    internal TConcrete? Instance
    {
        get => _instance;
        set
        {
            var publishChange = !EqualityComparer<TConcrete>.Default.Equals(_instance, value);
            _instance = value;
            if (publishChange) _invokeObservable.Send(value);
        }
    }

    public IObservable<TInterface?> ChangesStream => _invokeObservable;

    void IDisposable.Dispose()
    {
        _invokeObservable.Dispose();
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
        lock (_lock)
        {
            if (--_counter != 0) return;

            if (Instance is IDisposable disposable)
                disposable.Dispose();
            Instance = default;
        }
    }
}