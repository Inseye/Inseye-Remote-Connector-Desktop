// Module name: Shared
// File name: ObservableSubscriptionTracker.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Structs;

namespace EyeTrackerStreaming.Shared.Utility;

public sealed class ObservableSubscriptionTracker<T> : IObservable<T>, IDisposable
{
    private readonly object _lock = new();
    private readonly InvokeObservable<int> _subscribersObservable = new();
    private readonly IObservable<T> _tracked;
    private DisposeBool _disposed;
    private int _subscribersCount;

    public ObservableSubscriptionTracker(IObservable<T> tracked)
    {
        ArgumentNullException.ThrowIfNull(tracked, nameof(tracked));
        _tracked = tracked;
    }

    public IObservable<int> SubscribersCountObservable => _subscribersObservable;
    public int SubscribersCount => _subscribersCount;

    public void Dispose()
    {
        if (_disposed.PerformDispose()) _subscribersObservable.Dispose();
    }

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
}