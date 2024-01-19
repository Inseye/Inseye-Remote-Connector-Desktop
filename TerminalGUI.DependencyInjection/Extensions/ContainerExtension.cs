// Module name: TerminalGUI.DependencyInjection
// File name: ContainerExtension.cs
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

using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
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
        container.RegisterInstance<Toplevel>(new Window
        {
            BorderStyle = LineStyle.None
        });
        container.Register<IApplication, TerminalGuiApplication>(Lifestyle.Singleton);
        return container;
    }

    public static Container RegisterTerminalGuiApplication<TTop>(this Container container, Lifestyle lifestyle) where TTop : Toplevel
    {
        container.Register<TerminalGuiApplication>(lifestyle);
        container.Register<Toplevel, TTop>(lifestyle);
        container.Register<IApplication, TerminalGuiApplication>(lifestyle);
        return container;
    }

    public static Container RegisterTerminalGuiApplication<TTop>(this Container container) where TTop : Toplevel =>
        container.RegisterTerminalGuiApplication<TTop>(Lifestyle.Singleton);

    public static Container RegisterTerminalRouter(this Container container)
    {
        container.RegisterScopingRouterFor<TerminalGuiRouter>(Lifestyle.Singleton);
        container.RegisterConditional<IServiceProvider, ScopingServiceProvider>(Lifestyle.Singleton,
            context => context.HasConsumer && context.Consumer.ImplementationType == typeof(TerminalGuiRouter));
        return container;
    }
}