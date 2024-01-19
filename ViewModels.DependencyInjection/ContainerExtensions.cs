// Module name: ViewModels.DependencyInjection
// File name: ContainerExtensions.cs
// Last edit: 2024-1-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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
using EyeTrackingStreaming.ViewModels;
using SimpleInjector;

namespace ViewModels.DependencyInjection;

public static class ContainerExtensions
{
    public static Container RegisterAllViewModels(this Container container)
    {
        container.Register<SearchViewModel>(Lifestyle.Scoped);
        container.Register<StatusViewModel>(Lifestyle.Scoped);
        container.Register<CalibrationViewModel>(Lifestyle.Scoped);
        container.Register<ICalibrationHandler, RoutingCalibrationHandler>(Lifestyle.Scoped);
        return container;
    }
}