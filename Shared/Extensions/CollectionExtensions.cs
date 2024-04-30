// Module name: Shared
// File name: CollectionExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared.Extensions;

public static class CollectionExtensions
{
    public static void ForEachAggregateException<T>(this IEnumerable<T> collection, Action<T> callback)
    {
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));
        List<Exception>? exceptions = null;
        foreach (var element in collection)
            try
            {
                callback(element);
            }
            catch (Exception exception)
            {
                (exceptions ?? new List<Exception>(1)).Add(exception);
            }

        if (exceptions is not null)
            throw new AggregateException(exceptions);
    }

    public static void ForEachAggregateException<TCollection, TArg1>(this IEnumerable<TCollection> collection,
        Action<TCollection, TArg1> callback, TArg1 arg1)
    {
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));
        List<Exception>? exceptions = null;
        foreach (var element in collection)
            try
            {
                callback(element, arg1);
            }
            catch (Exception exception)
            {
                (exceptions ?? new List<Exception>(1)).Add(exception);
            }

        if (exceptions is not null)
            throw new AggregateException(exceptions);
    }
}