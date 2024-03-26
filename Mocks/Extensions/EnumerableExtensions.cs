// Module name: Mocks
// File name: EnumerableExtensions.cs
// Last edit: 2024-3-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using Grpc.Core;

namespace Mocks.Extensions;

public static class EnumerableExtensions
{
    public static AsyncServerStreamingCall<T> ToAsyncServerStreamingCall<T>(this IEnumerable<T> enumerable,
        Metadata? responseHeader = null, CancellationToken serverToken = default)
    {
        responseHeader ??= new Metadata();
        return new AsyncServerStreamingCall<T>(new AsyncStreamReaderFromEnumerable<T>(enumerable),
            Task.FromResult(responseHeader),
            () => serverToken.IsCancellationRequested ? Status.DefaultCancelled : Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
    }

    private class AsyncStreamReaderFromEnumerable<T> : IAsyncStreamReader<T>
    {
        public AsyncStreamReaderFromEnumerable(IEnumerable<T> source)
        {
            Source = source.GetEnumerator();
        }

        private IEnumerator<T> Source { get; }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.Run(() => Source.MoveNext(), cancellationToken);
        }

        public T Current => Source.Current;
    }
}