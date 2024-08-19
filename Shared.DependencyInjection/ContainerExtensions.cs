// Module name: Shared.DependencyInjection
// File name: ContainerExtensions.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using EyeTrackerStreaming.Shared.Configuration;
using EyeTrackerStreaming.Shared.Decorators;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.DependencyInjection.CrossScopedObject;
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
		container.Options.EnableAutoVerification = false;
		return container;
	}
    
    /// <summary>
    /// Registers service in container so it can be accessed between scopes safely.
    /// Service can be changed by requesting <c>IPublisher&lt;TService></c> singleton service.
    /// Serivce can be retrieved from shared cache with <c>IProvider&lt;TService></c> or requrest as scped service by directly requesting &lt;TService>.
    /// </summary>
    /// <param name="container">Container used to register service</param>
    /// <param name="validationInitializer">Optional factory method used to create proxy/mock object during container verfication</param>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <returns></returns>
    public static Container RegisterCrossScopeManagedService<TService>(
        this Container container, Func<TService>? validationInitializer = null)
        where TService : class
    {
        container.Register<CrossScopedObjectManager<TService>>(Lifestyle.Singleton);
        container.Register<IProvider<TService>, CrossScopedObjectManager<TService>>(
            Lifestyle.Singleton);
        container.Register<IPublisher<TService>, CrossScopedObjectManager<TService>>(Lifestyle.Singleton);
        container.Register(() => container.GetInstance<CrossScopedObjectManager<TService>>().Get()!, CrossScopedLifetime.Instance);
        if(validationInitializer != null)
            container.RegisterInitializer<IPublisher<TService>>(obj =>
            {
                if (container.IsVerifying)
                    obj.Publish(validationInitializer());
            });
        return container;
    }


	public static Container RegisterScopingRouterFor<TRouter>(this Container container, Lifestyle routersLifestyle)
		where TRouter : class, IRouter
	{
		container.Register<IRouter, ScopingRouter<TRouter>>(routersLifestyle);
		container.Register<TRouter>(routersLifestyle);
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