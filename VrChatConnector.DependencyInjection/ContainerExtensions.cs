// Module name: VrChatConnector.DependencyInjection
// File name: ContainerExtensions.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using SimpleInjector;
using VRChatConnector;

namespace VrChatConnector.DependencyInjection;

public static class ContainerExtensions
{
    public static Container RegisterVrChatConnector(this Container container)
    {
        container.Register<OscClient>(Lifestyle.Scoped);
        return container;
    }
}