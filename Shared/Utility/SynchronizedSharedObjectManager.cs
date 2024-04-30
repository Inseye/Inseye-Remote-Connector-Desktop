// Module name: Shared
// File name: SynchronizedSharedObjectManager.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Structs;

namespace EyeTrackerStreaming.Shared.Utility;

public class SynchronizedSharedObjectManager<T> : IDisposable where T : class
{
    private readonly object _lock = new();

    public SynchronizedSharedObjectManager(IFactory<T> factory)
    {
        Manager = new SharedObjectManager<T>(factory);
    }

    private SharedObjectManager<T> Manager { get; }

    public void Dispose()
    {
        lock (_lock)
        {
            Manager.Dispose();
        }
    }

    public SynchronizedSharedObjectToken Get()
    {
        lock (_lock)
        {
            return new SynchronizedSharedObjectToken(Manager.Get(), this);
        }
    }

    private void Return(SharedObjectManager<T>.SharedObjectToken token)
    {
        lock (_lock)
        {
            token.Dispose();
        }
    }

    public class SynchronizedSharedObjectToken : IDisposable
    {
        private DisposeBool _disposed;

        internal SynchronizedSharedObjectToken(SharedObjectManager<T>.SharedObjectToken token,
            SynchronizedSharedObjectManager<T> issuer)
        {
            Token = token;
            Issuer = issuer;
        }

        private SharedObjectManager<T>.SharedObjectToken Token { get; }
        private SynchronizedSharedObjectManager<T> Issuer { get; }

        public T Object => Token.Object;

        public void Dispose()
        {
            if (_disposed.PerformDispose())
                Issuer.Return(Token);
        }
    }
}