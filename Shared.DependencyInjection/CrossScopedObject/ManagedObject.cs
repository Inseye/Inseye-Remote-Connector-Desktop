// Module name: Shared.DependencyInjection
// File name: ManagedObject.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Structs;
using EyeTrackerStreaming.Shared.Utility;
using Microsoft.Extensions.Logging;
using SimpleInjector;

namespace Shared.DependencyInjection.CrossScopedObject;

internal class ManagedObject<TInterface, TConcrete, TVerification> : IDisposable, IProvider<TInterface>,
    IPublisher<TConcrete> where TConcrete : class, TInterface where TVerification : TInterface, new()
{
    private readonly bool _isVerifying;
    private readonly ILogger<ManagedObject<TInterface, TConcrete, TVerification>> _logger;
    private readonly ScopedObjectManager<TInterface, TConcrete> _scopedObjectManager;
    private DisposeBool _disposed;
    private readonly InvokeObservable<TInterface> _stream = new();

    public ManagedObject(ScopedObjectManager<TInterface, TConcrete> scopedObjectManager, Scope scope,
        ILogger<ManagedObject<TInterface, TConcrete, TVerification>> logger)
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

    IObservable<TInterface?> IProvider<TInterface>.ChangesStream()
    {
        return _disposed
            ? throw new ObjectDisposedException(nameof(IProvider<TInterface>))
            : _scopedObjectManager.ChangesStream;
    }


    void IPublisher<TConcrete>.Publish(TConcrete value)
    {
        _scopedObjectManager.Instance = value;
    }
}