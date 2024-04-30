// Module name: Shared
// File name: ServiceProviderExtensions.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared.Extensions;

public static class ServiceProviderExtensions
{
    public static T GetServiceRequired<T>(this IServiceProvider serviceProvider)
    {
        var result = serviceProvider.GetService(typeof(T));
        if (result is null)
            throw new NullReferenceException(
                $"Service provider {serviceProvider.GetType()} failed to resolve service of type: {typeof(T)}");
        return (T) result;
    }
}