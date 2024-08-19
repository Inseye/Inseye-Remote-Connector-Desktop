// Module name: Mocks.DependencyInjection
// File name: ContainerExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using SimpleInjector;

namespace Mocks.DependencyInjection;

public static class ContainerExtensions
{
    public static Container RegisterAllMocks(this Container container)
    {
        var prev = container.Options.AllowOverridingRegistrations;
        container.Options.AllowOverridingRegistrations = true;
        container.RegisterServiceOfferProviderMock();
        container.RegisterGrpcApiMock();
        container.Options.AllowOverridingRegistrations = prev;
        return container;
    }

    public static Container RegisterServiceOfferProviderMock(this Container container)
    {
        container.Register<IRemoteServiceOffersProvider, RemoteServiceOffersProviderMock>(Lifestyle.Scoped);
        return container;
    }

    public static Container RegisterGrpcApiMock(this Container container, Func<ServiceOffer, CancellationToken, ValueTask<IRemoteService>>? factoryFunction = null)
    {
        if (factoryFunction != null)
        {
            container.Register<IRemoteServiceFactory>(() =>
            {
                var mock = new RemoteServiceFactoryMock();
                mock.OnCreateRemoteService = factoryFunction;
                return mock;
            }, Lifestyle.Scoped);    
        }
        else
        {
            container.Register<IRemoteServiceFactory, RemoteServiceFactoryMock>(Lifestyle.Scoped);   
        }

        return container;
    }
}