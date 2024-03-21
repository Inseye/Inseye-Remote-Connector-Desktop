// Module name: TerminalGUI.DependencyInjection
// File name: ContainerExtension.cs
// Last edit: 2024-3-21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using Shared.DependencyInjection;
using SimpleInjector;
using Terminal.Gui;

namespace TerminalGUI.DependencyInjection.Extensions;

public static class ContainerExtension
{
    public static Container RegisterTerminalGui(this Container container)
    {
        return container
            .RegisterTerminalRouter()
            .RegisterTerminalGuiApplication();
    }


    public static Container RegisterTerminalGuiApplication(this Container container)
    {
        container.Register<TerminalGuiApplication>(Lifestyle.Singleton);
        container.Register(() => new Toplevel
        {
            BorderStyle = LineStyle.None
        }, Lifestyle.Singleton);
        container.Register<IUiThreadSynchronizationContext, TerminalGuiApplication>(Lifestyle.Singleton);
        container.Register<IApplication, TerminalGuiApplication>(Lifestyle.Singleton);
        return container;
    }

    public static Container RegisterTerminalGuiApplication<TTop>(this Container container, Lifestyle lifestyle)
        where TTop : Toplevel
    {
        container.Register<TerminalGuiApplication>(lifestyle);
        container.Register<Toplevel, TTop>(lifestyle);
        container.Register<IApplication, TerminalGuiApplication>(lifestyle);
        return container;
    }

    public static Container RegisterTerminalGuiApplication<TTop>(this Container container) where TTop : Toplevel
    {
        return container.RegisterTerminalGuiApplication<TTop>(Lifestyle.Singleton);
    }

    public static Container RegisterTerminalRouter(this Container container)
    {
        container.RegisterScopingRouterFor<TerminalGuiRouter>(Lifestyle.Singleton);
        container.RegisterConditional<IServiceProvider, ScopingServiceProvider>(Lifestyle.Singleton,
            context => context.HasConsumer && context.Consumer.ImplementationType == typeof(TerminalGuiRouter));
        return container;
    }

    /// <summary>
    ///     Wraps logger so GUI application will be shutdown when critical log is written.
    /// </summary>
    /// <param name="container">Container with services for terminal gui application</param>
    /// <returns>Container with registered circuit breaker.</returns>
    public static Container AddLoggerCircuitBreaker(this Container container)
    {
        container.RegisterDecorator(typeof(ILogger<>), typeof(TerminalGuiCircuitBreaker<>), Lifestyle.Singleton);
        return container;
    }

    private class TerminalGuiCircuitBreaker<T> : ILogger<T>
    {
        public TerminalGuiCircuitBreaker(ILogger<T> wrapped)
        {
            Wrapper = wrapped;
        }

        private ILogger<T> Wrapper { get; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Wrapper.Log(logLevel, eventId, state, exception, formatter);
            if (logLevel == LogLevel.Critical)
                Application.Invoke(() =>
                    throw new Exception("Critical error occured that terminated the application. Check log file."));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Wrapper.IsEnabled(logLevel);
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return Wrapper.BeginScope(state);
        }
    }
}