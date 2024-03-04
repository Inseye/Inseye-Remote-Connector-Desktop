// Module name: Shared
// File name: SimpleConsoleLogger.cs
// Last edit: 2024-2-29 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using Microsoft.Extensions.Logging;

namespace EyeTrackerStreaming.Shared.Utility;

/// <summary>
/// Utility logger for development, testing and debugging purposes. 
/// </summary>
/// <typeparam name="T">Logger consumer type</typeparam>
public sealed class SimpleConsoleLogger<T> : ILogger<T>
{
    public static readonly SimpleConsoleLogger<T> Instance = new(); 
    private SimpleConsoleLogger() {}
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        using var handle = SharedStringBuilderObjectPool.GetAutoDisposing();
        Console.WriteLine(handle.Object.Append(logLevel.ToString("G"))
            .Append(' ').Append(typeof(T).Name).Append(' ').Append( formatter(state, exception)));
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