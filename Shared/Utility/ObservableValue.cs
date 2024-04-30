// Module name: Shared
// File name: ObservableValue.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared.Utility;

public sealed class ObservableValue<T>(T initialValue, EqualityComparer<T> comparer) : IObservable<T>, IDisposable
{
    private readonly InvokeObservable<T> _invokeObservable = new();

    public ObservableValue(T initialValue) : this(initialValue, EqualityComparer<T>.Default)
    {
    }

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

    public void Dispose()
    {
        _invokeObservable.Dispose();
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        return _invokeObservable.Subscribe(observer);
    }
}