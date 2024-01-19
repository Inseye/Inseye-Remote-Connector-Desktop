// Module name: Shared.DependencyInjection
// File name: ContainerExtensions.cs
// Last edit: 2024-1-31 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using EyeTrackerStreaming.Shared.Decorators;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using Shared.DependencyInjection.CrossScopedObject;
using Shared.DependencyInjection.Interfaces;
using SimpleInjector;

namespace Shared.DependencyInjection;

public static class ContainerExtensions
{
    /// <summary>
    /// Sets 
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static Container SetDefaultOptions(this Container container)
    {
        container.Options.DefaultScopedLifestyle = ScopedLifestyle.Flowing;
        return container;
    }

    public static Container RegisterCrossScopeManagedService<TService, TValidationType>(this Container container)
        where TService : class where TValidationType : class, TService =>
        container.RegisterCrossScopeManagedService<TService, TService, TValidationType>();

    public static Container RegisterCrossScopeManagedService<TService, TImplementation, TValidationType>(
        this Container container)
        where TImplementation : class, TService
        where TService : class 
        where TValidationType : class, TImplementation
    {
        container.Register<TValidationType>(Lifestyle.Singleton);
        container.Register<ObjectManager<TService, TImplementation, TValidationType>>(Lifestyle.Singleton);
        container
            .Register<IProvider<TService>, ManagedObject<TService, TImplementation, TValidationType>>(
                Lifestyle.Scoped);
        container
            .Register<IPublisher<TImplementation>, ManagedObject<TService, TImplementation, TValidationType>>(
                Lifestyle.Scoped);
        return container;
    }
    

    public static Container RegisterScopingRouterFor<TRouter>(this Container container, Lifestyle routersLifestyle) where TRouter: class, IRouter
    {
        container.Register<IRouter, ScopingRouter<TRouter>>(routersLifestyle);
        container.Register<TRouter>(Lifestyle.Singleton);
        container.Register<IScopingRouter, ScopingRouter<TRouter>>(routersLifestyle);
        return container;
    }

    public static Container AddLogging(this Container container, Action<ILoggingBuilder> configure)
    {
        container.RegisterInstance<ILoggerFactory>(LoggerFactory.Create(configure));
        container.Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);
        return container;
    }

    public static Container AddRouterLogging(this Container container)
    {
        container.RegisterDecorator<IRouter, LoggingRouterDecorator>();
        return container;
    }
}