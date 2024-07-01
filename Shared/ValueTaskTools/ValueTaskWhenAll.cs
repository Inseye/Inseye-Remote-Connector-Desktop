// Module name: Shared
// File name: ValueTaskWhenAll.cs
// Last edit: 2024-06-27 10:03 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

namespace EyeTrackerStreaming.Shared.ValueTaskTools;

public static class ValueTaskWhenAll
{
	/// <summary>
	/// Allocation free (when no exceptions are thrown) when all implementation. 
	/// </summary>
	/// <param name="first">First task.</param>
	/// <param name="second">Second task</param>
	/// <typeparam name="TResult1">First task result type.</typeparam>
	/// <typeparam name="TResult2">Second task result type.</typeparam>
	/// <returns>Tuple with result from both tasks</returns>
	public async static ValueTask<(TResult1, TResult2)> WhenAll<TResult1, TResult2>(ValueTask<TResult1> first, ValueTask<TResult2> second)
	{
		(TResult1, TResult2) result = default;
		List<Exception>? exceptions = null;
		(result.Item1, exceptions) = await ExecuteHandlingInternal(first, exceptions);
		(result.Item2, exceptions) = await ExecuteHandlingInternal(second, exceptions);
		if (exceptions != null)
			throw new AggregateException(exceptions);
		return result;
	}

	/// <summary>
	/// Allocation free (when no exceptions are thrown) when all implementation. 
	/// </summary>
	/// <param name="tasks">Tasks to execute.</param>
	/// <typeparam name="TResult1">First task result type.</typeparam>
	/// <typeparam name="TResult2">Second task result type.</typeparam>
	/// <returns>Tuple with result from both tasks</returns>
	public static ValueTask<(TResult1, TResult2)> WhenAll<TResult1, TResult2>(this (ValueTask<TResult1>, ValueTask<TResult2>) tasks)
	{
		return WhenAll(tasks.Item1, tasks.Item2);
	}

	private static async ValueTask<(TResult, List<Exception>?)> ExecuteHandlingInternal<TResult>(ValueTask<TResult> task, List<Exception>? exceptionsContainer)
	{
		if (task.IsCompleted)
			return (task.Result, exceptionsContainer);
		try
		{
			var result = await task.ConfigureAwait(false);
			return (result, exceptionsContainer);
		}
		catch (Exception exception)
		{
			if (exceptionsContainer == null)
				return (default!, new List<Exception> { exception });
			exceptionsContainer.Add(exception);
			return (default!, exceptionsContainer);
		}
	}
}