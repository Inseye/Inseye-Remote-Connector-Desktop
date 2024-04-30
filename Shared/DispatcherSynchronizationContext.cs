// Module name: Shared
// File name: DispatcherSynchronizationContext.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Pooling;
using EyeTrackerStreaming.Shared.Structs;

namespace EyeTrackerStreaming.Shared;

using QueuedType = (SendOrPostCallback postCallback, object? state);

public sealed class DispatcherSynchronizationContext : SynchronizationContext, IDisposable
{
    private readonly int _associatedThreadId;
    private DisposeBool _disposed;

    public DispatcherSynchronizationContext(int associatedThreadId) : this(associatedThreadId,
        (ObjectRef<Queue<QueuedType>>)
        QueuePool<QueuedType>.Shared.Get())
    {
    }

    private DispatcherSynchronizationContext(int associatedThreadId,
        ObjectRef<Queue<QueuedType>> poolRef)
    {
        _associatedThreadId = associatedThreadId;
        PooledWorkQueueRef = poolRef;
    }

    private ObjectRef<Queue<(SendOrPostCallback postCallback, object? state)>> PooledWorkQueueRef { get; set; }

    public void Dispose()
    {
        if (!_disposed.PerformDispose())
            return;
        // move post callbacks to default synchronization context
        var context = new SynchronizationContext(); // ThreadPool synchronization context
        var workItems = StealScheduledWorkQueueInternal();
        foreach (var workItem in workItems) context.Post(workItem.postCallback, workItem.state);
    }

    public override void Post(SendOrPostCallback callback, object? state)
    {
        lock (PooledWorkQueueRef.Object)
        {
            PooledWorkQueueRef.Object.Enqueue((callback, state));
        }
    }

    public override void Send(SendOrPostCallback callback, object? state)
    {
        if (_associatedThreadId == Environment.CurrentManagedThreadId)
        {
            callback(state);
            return;
        }

        var wasExecuted = new Ref<bool>(false);
        Post(_ =>
        {
            callback(state);
            wasExecuted.Value = true;
        }, null);
        while (!wasExecuted.Value) Thread.Sleep(15);
    }

    /// <summary>
    ///     Steals queue with scheduled work.
    ///     After use the queue can be returned to
    ///     <see cref="QueuePool{T}.Shared">QueuePool&lt;(SendOrPostCallback postCallback, object? state)&gt;.Shared</see>
    /// </summary>
    /// <returns>Queue with queued work</returns>
    public Queue<QueuedType> StealScheduledWorkQueue()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(DispatcherSynchronizationContext));
        return StealScheduledWorkQueueInternal();
    }

    private Queue<QueuedType> StealScheduledWorkQueueInternal()
    {
        Queue<QueuedType> returned;
        var newQueue = QueuePool<QueuedType>.Shared.Get();
        lock (PooledWorkQueueRef.Object)
        {
            returned = PooledWorkQueueRef.Object;
            PooledWorkQueueRef.Object = newQueue;
        }

        return returned;
    }


    public override SynchronizationContext CreateCopy()
    {
        return new DispatcherSynchronizationContext(_associatedThreadId, PooledWorkQueueRef);
    }
}