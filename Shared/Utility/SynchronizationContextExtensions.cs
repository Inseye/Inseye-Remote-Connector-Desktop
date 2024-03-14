// Module name: Shared
// File name: TaskHelpers.cs
// Last edit: 2024-3-13 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

namespace EyeTrackerStreaming.Shared.Utility;

public static class SynchronizationContextExtensions
{
    /// <summary>
    /// Asserts that task returned from task factory will be started on specific synchronization context
    /// </summary>
    /// <param name="context">Desired synchronization context</param>
    /// <param name="taskFactory">Function that starts task</param>
    /// <returns>Started task on specified synchronization context</returns>
    public async static Task Run(this SynchronizationContext? context, Func<Task> taskFactory)
    {
        if (SynchronizationContext.Current != context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
            await Task.Yield();
        }
        await taskFactory();
    }
    
    /// <inheritdoc cref="Run"/>
    public async static Task<T> Run<T>(this SynchronizationContext? context, Func<Task<T>> taskFactory)
    {
        if (SynchronizationContext.Current != context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
            await Task.Yield();
        }
        return await taskFactory();
    }
    /// <inheritdoc cref="Run"/>
    /// <param name="arg1">First argument passed to task factory</param>
    public async static Task<TResult> Run<TResult, TArg1>(this SynchronizationContext? context,
        Func<TArg1, Task<TResult>> taskFactory, TArg1 arg1)
    {
        if (SynchronizationContext.Current != context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
            await Task.Yield();
        }
        return await taskFactory(arg1);
    }
    /// <summary>
    /// Asserts that task returned from task factory will be run on default thread pool scheduler
    /// </summary>
    /// <param name="taskFactory">Function that starts task</param>
    /// <returns>Started task on specified synchronization context</returns>
    public static Task RunOnNull(Func<Task> taskFactory) => Run(null, taskFactory);
    
    /// <inheritdoc cref="RunOnNull"/>
    public static Task<T> RunOnNull<T>(Func<Task<T>> taskFactory) => Run(null, taskFactory);
    
    /// <inheritdoc cref="RunOnNull"/>
    /// <param name="arg1">First argument passed to task factory</param>
    public static Task<TResult> RunOnNull<TResult, TArg1>(Func<TArg1, Task<TResult>> taskFactory, TArg1 arg1) =>
        Run(null, taskFactory, arg1);
    
    /// <summary>
    /// Switches to specified synchronization context.
    /// </summary>
    /// <param name="context">Desired synchronization context</param>
    /// <returns>Task that when finished asserts that rest of execution will occur on desired synchronization context</returns>
    public static ValueTask SwitchTo(this SynchronizationContext? context)
    {
        if (SynchronizationContext.Current == context)
            return ValueTask.CompletedTask;
        SynchronizationContext.SetSynchronizationContext(context);
        return Yield();
    }

    private static async ValueTask Yield()
    {
        await Task.Yield();
    }
}