// Module name: Shared
// File name: SimpleConsoleLogger.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace EyeTrackerStreaming.Shared.Utility;

/// <summary>
///     Utility logger for development, testing and debugging purposes.
/// </summary>
/// <typeparam name="T">Logger consumer type</typeparam>
public sealed class SimpleConsoleLogger<T> : ILogger<T>
{
    public static readonly SimpleConsoleLogger<T> Instance = new();

    private SimpleConsoleLogger()
    {
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        using var handle = StringBuilderPool.Shared.GetAutoDisposing();
        Console.WriteLine(handle.Object.Append(logLevel.ToString("G"))
            .Append(' ').Append(typeof(T).Name).Append(' ').Append(formatter(state, exception)));
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}