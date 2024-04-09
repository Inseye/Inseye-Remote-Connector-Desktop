// Module name: Shared
// File name: InvokeObservable.cs
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

using EyeTrackerStreaming.Shared.Extensions;
using EyeTrackerStreaming.Shared.Pooling;
using EyeTrackerStreaming.Shared.Structs;

namespace EyeTrackerStreaming.Shared.Utility;

public class InvokeObservable<T> : IObservable<T>, IDisposable
{
    private ChangeList? _changeList;
    private bool _isIterating;
    private readonly HashSet<IObserver<T>> _observers = new();
    private DisposeBool _disposed;


    public void Dispose()
    {
        if (_disposed.PerformDispose())
            return;
        Complete();
        if (_changeList != null)
        {
            ChangeListPool.Shared.Return(_changeList);
            _changeList = null;
        }
        _observers.Clear();
    }

    IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
    {
        return Subscribe(observer);
    }

    public InvokeObservableSubscriptionDisposer Subscribe(IObserver<T> observer)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_observers)
        {
            if (_isIterating)
                GetChangeList().Add((observer, ChangeList.Change.Added));
            else
                _observers.Add(observer);

            return new InvokeObservableSubscriptionDisposer(this, observer);
        }
    }

    public void Send(T value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_observers)
        {
            using (new BoolToggle(ref _isIterating))
                try
                {
                    _observers.ForEachAggregateException((observer, val) => observer.OnNext(val), value);
                }
                finally
                {
                    CheckChangeList();
                }
        }
    }

    public void SendError(Exception exception)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(InvokeObservable<T>));
        lock (_observers)
        {
            using (new BoolToggle(ref _isIterating))
                try
                {
                    _observers.ForEachAggregateException((observer, val) => observer.OnError(val), exception);
                }
                finally
                {
                    CheckChangeList();
                }
        }
    }

    public void Complete()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_observers)
        {
            using (new BoolToggle(ref _isIterating))
                try
                {
                    _observers.ForEachAggregateException(observer => observer.OnCompleted());
                }
                finally
                {
                    CheckChangeList();
                }
        }
    }

    private void RemoveSubscriber(IObserver<T> subscriber)
    {
        if (_disposed)
            return;
        lock (_observers)
        {
            if (_isIterating)
                GetChangeList().Add((subscriber, ChangeList.Change.Removed));
            else
                _observers.Remove(subscriber);
        }
    }

    private void CheckChangeList()
    {
        if(_disposed)
            return;
        if (_changeList == null)
            return;

        foreach (var change in _changeList)
        {
            if (change.change == ChangeList.Change.Added)
                _observers.Add((IObserver<T>) change.obj);
            else
                _observers.Remove((IObserver<T>) change.obj);
        }

        ChangeListPool.Shared.Return(_changeList);
        _changeList = null;
    }

    private ChangeList GetChangeList()
    {
        return _changeList ??= ChangeListPool.Shared.Get();
    }

    public struct InvokeObservableSubscriptionDisposer : IDisposable
    {
        private readonly InvokeObservable<T> _observable;
        private readonly IObserver<T> _observer;
        private bool _needsDisposing;

        internal InvokeObservableSubscriptionDisposer(InvokeObservable<T> observable, IObserver<T> observer)
        {
            _observable = observable;
            _observer = observer;
            _needsDisposing = true;
        }

        public void Dispose()
        {
            if (_needsDisposing)
            {
                _needsDisposing = false;
                try
                {
                    _observer.OnCompleted();
                }
                finally
                {
                    _observable.RemoveSubscriber(_observer);
                }
            }
        }
    }
}