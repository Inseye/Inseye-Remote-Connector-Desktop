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

namespace EyeTrackerStreaming.Shared.Utility;

public class InvokeObservable<T> : IObservable<T>, IDisposable
{
    private readonly HashSet<IObserver<T>> _observers = new();
    private bool _isFinished;


    public void Dispose()
    {
        if (_isFinished)
            return;
        Complete();
    }

    IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
    {
        return Subscribe(observer);
    }

    public InvokeObservableSubscriptionDisposer Subscribe(IObserver<T> observer)
    {
        lock (_observers)
        {
            if (_isFinished)
            {
                observer.OnCompleted();
                return default;
            }

            _observers.Add(observer);
            return new InvokeObservableSubscriptionDisposer(this, observer);
        }
    }

    public void Send(T value)
    {
        if (_isFinished)
            throw new ObjectDisposedException(nameof(InvokeObservable<T>));
        lock (_observers)
        {
            _observers.ForEachAggregateException((observer, val) => observer.OnNext(val), value);
        }
    }

    public void SendError(Exception exception)
    {
        if (_isFinished)
            throw new ObjectDisposedException(nameof(InvokeObservable<T>));
        lock (_observers)
        {
            _observers.ForEachAggregateException((observer, val) => observer.OnError(val), exception);
        }
    }

    public void Complete()
    {
        if (_isFinished)
            return;
        lock (_observers)
        {
            if (_isFinished)
                return;
            _isFinished = true;
            try
            {
                _observers.ForEachAggregateException(observer => observer.OnCompleted());
            }
            finally
            {
                _observers.Clear();
            }
        }
    }

    private void RemoveSubscriber(IObserver<T> subscriber)
    {
        if (_isFinished)
            return;
        lock (_observers)
        {
            _observers.Remove(subscriber);
        }
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