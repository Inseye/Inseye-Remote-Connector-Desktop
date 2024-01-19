﻿// Module name: Shared.DependencyInjection
// File name: ObjectManager.cs
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

using EyeTrackerStreaming.Shared.Utility;
using SimpleInjector;

namespace Shared.DependencyInjection.CrossScopedObject;

internal class ObjectManager<TInterface, TConcrete, TValidation>(TValidation validationObject, Container container) : IDisposable
    where TConcrete : TInterface
    where TValidation : TConcrete
{
    private readonly object _lock = new();
    private int _counter;
    private TConcrete? _instance;

    internal TConcrete? Instance
    {
        get => container.IsVerifying ? validationObject : _instance;
        set
        {
            var publishChange = !EqualityComparer<TConcrete>.Default.Equals(_instance, value);
            _instance = value;
            if (publishChange) {
                _invokeObservable.Send(value);
            }
        }
    }

    private readonly InvokeObservable<TInterface?> _invokeObservable = new();
    public IObservable<TInterface?> ChangesStream => _invokeObservable;

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

    void IDisposable.Dispose()
    {
        _invokeObservable.Dispose();
    }
}