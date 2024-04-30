// Module name: Shared.DependencyInjection
// File name: DisposableExtensions.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using SimpleInjector;

namespace Shared.DependencyInjection;

public static class DisposableExtensions
{
    public static T DisposeWith<T>(this T obj, Scope scope) where T : IDisposable
    {
        ArgumentNullException.ThrowIfNull(scope, nameof(scope));
        scope.RegisterForDisposal(obj);
        return obj;
    }
}