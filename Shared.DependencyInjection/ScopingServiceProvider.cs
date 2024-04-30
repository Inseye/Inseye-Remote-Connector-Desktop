// Module name: Shared.DependencyInjection
// File name: ScopingServiceProvider.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Shared.DependencyInjection.Interfaces;

namespace Shared.DependencyInjection;

public class ScopingServiceProvider(IScopingRouter scopingRouter) : IServiceProvider
{
    public object? GetService(Type serviceType)
    {
        return scopingRouter.CurrentRouteScope.GetInstance(serviceType);
    }
}