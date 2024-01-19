// Module name: Shared.DependencyInjection
// File name: ManagedObject.cs
// Last edit: 2024-2-20 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;

namespace Shared.DependencyInjection.CrossScopedObject;

internal class ManagedObject<TInterface, TConcrete, TValidation> : IDisposable, IProvider<TInterface>,
    IPublisher<TConcrete> where TConcrete : TInterface where TValidation : TConcrete
{
    private readonly ObjectManager<TInterface, TConcrete, TValidation> _objectManager;
    private InvokeObservable<TInterface> _stream = new();
    private DisposeBool _disposed;

    public ManagedObject(ObjectManager<TInterface, TConcrete, TValidation> objectManager)
    {
        _objectManager = objectManager;
        objectManager.IncrementCounter();
    }

    void IDisposable.Dispose()
    {
        if (!_disposed.PerformDispose())
            return;
        _stream.Dispose();
        _objectManager.DecrementCounter();
    }

    TInterface IProvider<TInterface>.Get()
    {
        var instance = _objectManager.Instance;
        if (instance == null)
            throw new Exception($"Failed to provide service of type: {typeof(TInterface)}");
        return instance;
    }

    IObservable<TInterface?> IProvider<TInterface>.ChangesStream() => _disposed
        ? throw new ObjectDisposedException(nameof(IProvider<TInterface>))
        : _objectManager.ChangesStream;


    void IPublisher<TConcrete>.Publish(TConcrete value)
    {
        _objectManager.Instance = value;
    }
}