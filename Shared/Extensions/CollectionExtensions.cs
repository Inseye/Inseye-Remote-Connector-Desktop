// Module name: Shared
// File name: CollectionExtensions.cs
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

namespace EyeTrackerStreaming.Shared.Extensions;

public static class CollectionExtensions
{
    public static void ForEachAggregateException<T>(this IEnumerable<T> collection, Action<T> callback)
    {
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));
        List<Exception>? exceptions = null;
        foreach (var element in collection)
        {
            try
            {
                callback(element);
            }
            catch (Exception exception)
            {
                (exceptions ?? new List<Exception>(1)).Add(exception);
            }
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
        {
            try
            {
                callback(element, arg1);
            }
            catch (Exception exception)
            {
                (exceptions ?? new List<Exception>(1)).Add(exception);
            }
        }

        if (exceptions is not null)
            throw new AggregateException(exceptions);
    }
}