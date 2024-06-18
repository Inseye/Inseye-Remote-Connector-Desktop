// Module name: Shared.DependencyInjection
// File name: ContainerExtensions.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Configuration;
using EyeTrackerStreaming.Shared.Decorators;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.DependencyInjection.CrossScopedObject;
using Shared.DependencyInjection.Interfaces;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Void = EyeTrackerStreaming.Shared.NullObjects.Void;

namespace Shared.DependencyInjection;

public static class ContainerExtensions
{
    /// <summary>
    ///     Sets
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static Container SetDefaultOptions(this Container container)
    {
        container.Options.DefaultScopedLifestyle = ScopedLifestyle.Flowing;
        return container;
    }

    public static Container RegisterCrossScopeManagedService<TService, TValidationType>(this Container container)
        where TService : class where TValidationType : class, TService, new()
    {
        return container.RegisterCrossScopeManagedService<TService, TService, TValidationType>();
    }

    public static Container RegisterCrossScopeManagedService<TService, TImplementation, TValidationType>(
        this Container container)
        where TService : class
        where TImplementation : class, TService
        where TValidationType : class, TService, new()
    {
        container.Register<TValidationType>(Lifestyle.Singleton);
        container.Register<ScopedObjectManager<TService, TImplementation>>(Lifestyle.Singleton);
        container.Register<IProvider<TService>, ManagedObject<TService, TImplementation, TValidationType>>(
            Lifestyle.Scoped);
        container.Register<IPublisher<TImplementation>, ManagedObject<TService, TImplementation, TValidationType>>(
            Lifestyle.Scoped);
        return container;
    }

    public static Container RegisterCrossContainer<TService>(this Container targetContainer,
        Container sourceContainer, Lifestyle lifestyle) where TService : class
    {
        if (!sourceContainer.IsLocked)
            throw new InvalidOperationException(
                "Source container must be locked (ready to use) before registering cross container service");


        if (lifestyle is SingletonLifestyle)
        {
            var instance = sourceContainer.GetInstance<TService>();
            targetContainer.RegisterInstance(instance);
        }
        else if (lifestyle is ScopedLifestyle scopedLifestyle)
        {
            targetContainer.Register(() =>
            {
                var targetScope = scopedLifestyle.GetCurrentScope(targetContainer);
                var producer = sourceContainer.GetRegistration<TService>();
                return (TService) producer!.GetInstance(targetScope);
            }, lifestyle);
        }
        else
        {
            targetContainer.Register(sourceContainer.GetInstance<TService>, lifestyle);
        }

        return targetContainer;
    }


    public static Container RegisterScopingRouterFor<TRouter>(this Container container, Lifestyle routersLifestyle)
        where TRouter : class, IRouter
    {
        container.Register<IRouter, ScopingRouter<TRouter>>(routersLifestyle);
        container.Register<TRouter>(Lifestyle.Singleton);
        container.Register<IScopingRouter, ScopingRouter<TRouter>>(routersLifestyle);
        return container;
    }

    public static Container AddLogging(this Container container, Action<ILoggingBuilder> configure)
    {
        container.Register(() => LoggerFactory.Create(configure), Lifestyle.Singleton);
        container.Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);
        container.Register<ILogger, Logger<Void>>();
        return container;
    }

    public static Container AddRouterLogging(this Container container)
    {
        container.RegisterDecorator<IRouter, LoggingRouterDecorator>();
        return container;
    }

    public static Container RegisterScopedForSingleton<TService, TSingleton>(this Container container)
        where TService : class
    {
        container.RegisterConditional<TService>(Lifestyle.Singleton.CreateRegistration(() =>
        {
            var scope = new Scope(container);
            container.ContainerScope.RegisterForDisposal(scope);
            return scope.GetInstance<TService>();
        }, container), ctx => ctx.HasConsumer && ctx.Consumer.ImplementationType == typeof(TSingleton));
        return container;
    }

    public static Container AddOptions<T>(this Container container, T value) where T : class
    {
        container.RegisterInstance<IOptions<T>>(new ConstOption<T>(value));
        return container;
    }
}