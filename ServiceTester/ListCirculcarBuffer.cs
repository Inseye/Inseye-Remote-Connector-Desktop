// Module name: ServiceTester
// File name: ListCicrulcarBuffer.cs
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

using System.Collections;
using CircularBuffer;

namespace ServiceTester;

public class ListCirculcarBuffer<T> : CircularBuffer<T>, IList<T>, IList // lazy and sloppy implementation, just for ServiceTester
{
    public ListCirculcarBuffer(int capacity) : base(capacity)
    {
    }

    public ListCirculcarBuffer(int capacity, T[] items) : base(capacity, items)
    {
    }

    public void Add(T item)
    {
        PushFront(item);
    }

    public bool Contains(T item)
    {
        var compararer = EqualityComparer<T>.Default;
        foreach (var element in this)
        {
            if (compararer.Equals(element, item))
                return true;
        }

        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = base[i];
        }
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    public int Count => Size;
    public bool IsSynchronized { get; }
    public object SyncRoot { get; }
    public bool IsReadOnly => false;

    public object? this[int index]
    {
        get => base[index];
        set => base[index] = (T) value;
    }

    public int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < Size; i++)
        {
            if (comparer.Equals(base[i], item))
                return i;
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public int Add(object? value)
    {
        throw new NotImplementedException();
    }

    public bool Contains(object? value)
    {
        throw new NotImplementedException();
    }

    public int IndexOf(object? value)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, object? value)
    {
        throw new NotImplementedException();
    }

    public void Remove(object? value)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public bool IsFixedSize => true;
}