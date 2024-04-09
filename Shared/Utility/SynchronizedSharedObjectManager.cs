// Module name: Shared
// File name: SynchronizedSharedObjectManager.cs
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

using System.Dynamic;
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
    
    public SynchronizedSharedObjectToken Get()
    {
        lock (_lock)
        {
            return new (Manager.Get(), this);
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
        private SharedObjectManager<T>.SharedObjectToken Token { get; }
        private SynchronizedSharedObjectManager<T> Issuer { get; }

        internal SynchronizedSharedObjectToken(SharedObjectManager<T>.SharedObjectToken token,
            SynchronizedSharedObjectManager<T> issuer)
        {
            Token = token;
            Issuer = issuer;
        }

        public T Object => Token.Object;

        public void Dispose()
        {
            if(_disposed.PerformDispose())
                Issuer.Return(Token);
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            Manager.Dispose();
        }
    }
}