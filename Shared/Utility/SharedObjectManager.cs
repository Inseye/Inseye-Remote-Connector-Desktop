// Module name: Shared
// File name: SharedObjectManager.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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