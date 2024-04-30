// Module name: TerminalGUI
// File name: ToSynchronizationContextScheduler.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace TerminalGUI;

public class ToSynchronizationContextScheduler : LocalScheduler
{
    private readonly SynchronizationContext _context;

    public ToSynchronizationContextScheduler(SynchronizationContext context)
    {
        _context = context;
    }

    public override IDisposable Schedule<TState>(
        TState state, TimeSpan dueTime,
        Func<IScheduler, TState, IDisposable> action)
    {
        IDisposable PostOnMainLoop()
        {
            var composite = new CompositeDisposable(2);
            var cancellation = new CancellationDisposable();
            _context.Post(_ =>
            {
                if (!cancellation.Token.IsCancellationRequested)
                    composite.Add(action(this, state));
            }, null);
            composite.Add(cancellation);
            return composite;
        }

        IDisposable PostOnMainLoopAsTimeout()
        {
            var composite = new CompositeDisposable(2);
            var cancellation = new CancellationDisposable();
            composite.Add(cancellation);
            Func<IScheduler, TState, IDisposable> threadPoolAction = (_, _) =>
            {
                if (!cancellation.Token
                        .IsCancellationRequested) // skip scheduling to context if action is already cancelled
                    _context.Post(_ =>
                    {
                        if (!cancellation.Token
                                .IsCancellationRequested) // skip invoking on context if action is already cancelled
                            action(this, state);
                    }, null);
                return composite;
            };
            var threadPoolDispose =
                ThreadPoolScheduler.Instance.Schedule(state, dueTime,
                    threadPoolAction); // schedule continuation on dispatcher after timeout
            composite.Add(threadPoolDispose);
            return composite;
        }

        return dueTime == TimeSpan.Zero
            ? PostOnMainLoop()
            : PostOnMainLoopAsTimeout();
    }
}