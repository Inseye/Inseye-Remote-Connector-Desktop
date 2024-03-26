// Module name: TerminalGUI
// File name: ToSynchronizationContextScheduler.cs
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