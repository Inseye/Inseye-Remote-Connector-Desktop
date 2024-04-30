// Module name: Shared.DependencyInjection
// File name: OuterScopeLifestyle.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reflection;
using SimpleInjector;

namespace Shared.DependencyInjection;

public class OuterScopedLifestyle : ScopedLifestyle
{
    // Unfortunately, the Scope.ParentScope property is internal in Simple Injector v4.0.
    // We need to use reflection to get to it.
    private static readonly PropertyInfo ParentScopeProperty =
        typeof(Scope).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(p => p.Name == "ParentScope");

    public OuterScopedLifestyle() : base("Outer Scoped")
    {
    }

    // By making the length of this lifestyle one bigger than Lifestyle.Scoped, prevent
    // OuterScopedLifestyle instances from depending on Lifestyle.Scope instances,
    // which could obviously lead to problems.
    public override int Length => Scoped.Length + 1;

    protected override Func<Scope?> CreateCurrentScopeProvider(Container container)
    {
        return () => GetOuterScope(Scoped.GetCurrentScope(container));
    }

    protected override Scope? GetCurrentScopeCore(Container container)
    {
        return GetOuterScope(Scoped.GetCurrentScope(container));
    }

    // Walk the scope-stack to find the outer-most scope.
    private Scope? GetOuterScope(Scope? scope)
    {
        if (scope == null) return null;

        var parentScope = GetParentScope(scope);

        while (parentScope != null)
        {
            scope = parentScope;
            parentScope = GetParentScope(scope);
        }

        return scope;
    }

    private Scope GetParentScope(Scope? scope)
    {
        return (Scope) ParentScopeProperty.GetValue(scope) ??
               throw new NullReferenceException("Failed to resolve parent scope.");
    }
}