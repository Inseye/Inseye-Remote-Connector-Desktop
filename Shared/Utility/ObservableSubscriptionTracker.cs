// Module name: Shared
// File name: ObservableSubscriptionTracker.cs
// Last edit: 2024-2-19 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using EyeTrackerStreaming.Shared.Structs;

namespace EyeTrackerStreaming.Shared.Utility;

public sealed class ObservableSubscriptionTracker<T> : IObservable<T>, IDisposable
{
    private readonly object _lock = new();
    private int _subscribersCount;
    private DisposeBool _disposed;
    private readonly InvokeObservable<int> _subscribersObservable = new();
    private readonly IObservable<T> _tracked;

    public ObservableSubscriptionTracker(IObservable<T> tracked)
    {
        ArgumentNullException.ThrowIfNull(tracked, nameof(tracked));
        _tracked = tracked;
    }

    public IObservable<int> SubscribersCountObservable => _subscribersObservable;
    public int SubscribersCount => _subscribersCount;

    public IDisposable Subscribe(IObserver<T> observer)
    {
        lock (_lock)
        {
            var handle = new TrackerHandle(this, _tracked.Subscribe(observer));
            _subscribersObservable.Send(++_subscribersCount);
            return handle;
        }
    }

    private void DecrementCounter()
    {
        lock (_lock)
        {
            _subscribersObservable.Send(--_subscribersCount);
        }
    }
    
    private class TrackerHandle(ObservableSubscriptionTracker<T> parent, IDisposable targetDisposable) : IDisposable
    {
        private DisposeBool _disposed;
     
        public void Dispose()
        {
            if (!_disposed.PerformDispose()) return;
            try
            {
                targetDisposable.Dispose();
            }
            finally
            {
                parent.DecrementCounter();
            }
        }
    }

    public void Dispose()
    {
        if (_disposed.PerformDispose())
        {
            _subscribersObservable.Dispose();
        }
    }
}