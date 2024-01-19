// Module name: Shared
// File name: ObservableValue.cs
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

namespace EyeTrackerStreaming.Shared.Utility;

public sealed class ObservableValue<T>(T initialValue, EqualityComparer<T> comparer) : IObservable<T>, IDisposable
{
    private readonly InvokeObservable<T> _invokeObservable = new();
    public ObservableValue(T initialValue) : this(initialValue, EqualityComparer<T>.Default) {}
    
    public T Value
    {
        get => initialValue;
        set
        {
            if (comparer.Equals(initialValue, value))
                return;
            initialValue = value;
            _invokeObservable.Send(value);
        }
    }
    
    public IDisposable Subscribe(IObserver<T> observer)
    {
        return _invokeObservable.Subscribe(observer);
    }

    public void Dispose()
    {
        _invokeObservable.Dispose();
    }
}