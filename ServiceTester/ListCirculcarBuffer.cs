// Module name: ServiceTester
// File name: ListCirculcarBuffer.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections;
using CircularBuffer;

namespace ServiceTester;

public class
    ListCirculcarBuffer<T> : CircularBuffer<T>, IList<T>,
    IList // lazy and sloppy implementation, just for ServiceTester
{
    public ListCirculcarBuffer(int capacity) : base(capacity)
    {
    }

    public ListCirculcarBuffer(int capacity, T[] items) : base(capacity, items)
    {
    }

    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    public bool IsSynchronized { get; }
    public object SyncRoot { get; }

    public object? this[int index]
    {
        get => base[index];
        set => base[index] = (T) value;
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

    public bool IsFixedSize => true;

    public void Add(T item)
    {
        PushFront(item);
    }

    public bool Contains(T item)
    {
        var compararer = EqualityComparer<T>.Default;
        foreach (var element in this)
            if (compararer.Equals(element, item))
                return true;

        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (var i = 0; i < Count; i++) array[arrayIndex + i] = base[i];
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    public int Count => Size;
    public bool IsReadOnly => false;

    public int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < Size; i++)
            if (comparer.Equals(base[i], item))
                return i;

        return -1;
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }
}