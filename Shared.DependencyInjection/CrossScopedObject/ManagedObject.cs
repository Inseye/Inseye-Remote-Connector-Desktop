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
using Microsoft.Extensions.Logging;
using SimpleInjector;

namespace Shared.DependencyInjection.CrossScopedObject;

internal class ManagedObject<TInterface, TConcrete, TVerification> : IDisposable, IProvider<TInterface>,
    IPublisher<TConcrete> where TConcrete : class, TInterface where TVerification: TInterface, new()
{
    private readonly ScopedObjectManager<TInterface, TConcrete> _scopedObjectManager;
    private InvokeObservable<TInterface> _stream = new();
    private DisposeBool _disposed;
    private readonly bool _isVerifying;
    private readonly ILogger<ManagedObject<TInterface, TConcrete, TVerification>> _logger;
    public ManagedObject(ScopedObjectManager<TInterface, TConcrete> scopedObjectManager, Scope scope, ILogger<ManagedObject<TInterface, TConcrete, TVerification>> logger)
    {
        _scopedObjectManager = scopedObjectManager;
        scopedObjectManager.IncrementCounter();
        _isVerifying = scope.Container!.IsVerifying;
        _logger = logger;
    }

    void IDisposable.Dispose()
    {
        if (!_disposed.PerformDispose())
            return;
        _stream.Dispose();
        _scopedObjectManager.DecrementCounter();
    }

    public bool TryGet(out TInterface value)
    {
        if (_scopedObjectManager.Instance != null)
        {
            value = _scopedObjectManager.Instance;
            return true;
        }

        value = default!;
        return false;
    }

    TInterface IProvider<TInterface>.Get()
    {
        if (_isVerifying)
        {
            _logger.LogWarning($"Creating validation instance of {nameof(TVerification)}");
            return new TVerification();
        }

        var instance = _scopedObjectManager.Instance;
        if (instance == null)
            throw new Exception($"Failed to provide service of type: {typeof(TInterface)}");
        return instance;
    }

    IObservable<TInterface?> IProvider<TInterface>.ChangesStream() => _disposed
        ? throw new ObjectDisposedException(nameof(IProvider<TInterface>))
        : _scopedObjectManager.ChangesStream;


    void IPublisher<TConcrete>.Publish(TConcrete value)
    {
        _scopedObjectManager.Instance = value;
    }
}