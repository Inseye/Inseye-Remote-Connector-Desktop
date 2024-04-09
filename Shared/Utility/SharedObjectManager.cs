// Module name: Shared
// File name: SharedObjectManager.cs
// Last edit: 2024-3-20 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Structs;

namespace EyeTrackerStreaming.Shared.Utility;

public class SharedObjectManager<T> : IDisposable where T : class
{
    private DisposeBool _disposed;

    public SharedObjectManager(IFactory<T> factory)
    {
        Factory = factory;
    }

    private IFactory<T> Factory { get; }
    private T Instance { get; set; } = null!;
    private HashSet<SharedObjectToken> Tokens { get; } = new();

    public void Dispose()
    {
        if (!_disposed.PerformDispose()) return;
        foreach (var token in Tokens)
            token.Dispose();
        DisposeInstance();
    }

    public SharedObjectToken Get()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SharedObjectManager<T>));
        if (Tokens.Count == 0)
            Instance = Factory.Create();
        var token = new SharedObjectToken(this);
        Tokens.Add(token);
        return token;
    }

    private void Return(SharedObjectToken token)
    {
        if (_disposed)
            return;
        Tokens.Remove(token);
        if (Tokens.Count != 0)
            return;
        DisposeInstance();
    }

    private void DisposeInstance()
    {
        if (Instance is IDisposable disposable)
            disposable.Dispose();
        Instance = null!;
    }

    public class SharedObjectToken : IDisposable
    {
        private DisposeBool _disposed;

        internal SharedObjectToken(SharedObjectManager<T> parent)
        {
            SharedObjectManager = parent;
        }

        private SharedObjectManager<T> SharedObjectManager { get; }

        public T Object
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(SharedObjectToken));
                return SharedObjectManager.Instance;
            }
        }

        public void Dispose()
        {
            if (!_disposed.PerformDispose())
                return;
            SharedObjectManager.Return(this);
        }
    }
}