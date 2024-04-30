// Module name: Mocks
// File name: EnumerableExtensions.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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