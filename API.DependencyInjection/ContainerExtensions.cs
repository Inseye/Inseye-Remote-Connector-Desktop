// Module name: API.DependencyInjection
// File name: ContainerExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using API.Dnssd;
using API.Grpc;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using SimpleInjector;

namespace gRPC.DependencyInjection;

public static class ContainerExtensions
{
    public static Container RegisterGrpcApi(this Container container)
    {
        container.Register<IRemoteServiceFactory, GrpcRemoteServiceFactory>(Lifestyle.Singleton);
        return container;
    }

    public static Container RegisterZeroconfServiceOfferProvider(this Container container)
    {
        container.Register<IRemoteServiceOffersProvider, ZeroconfServiceProvider>(Lifestyle.Scoped);
        return container;
    }
}