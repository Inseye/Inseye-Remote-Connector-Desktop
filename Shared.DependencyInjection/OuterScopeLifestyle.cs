// Module name: Shared.DependencyInjection
// File name: OuterScopeLifestyle.cs
// Last edit: 2024-1-29 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Reflection;
using SimpleInjector;

namespace Shared.DependencyInjection;

public class OuterScopedLifestyle : ScopedLifestyle
{
    public OuterScopedLifestyle() : base("Outer Scoped")
    {
    }

    // By making the length of this lifestyle one bigger than Lifestyle.Scoped, prevent
    // OuterScopedLifestyle instances from depending on Lifestyle.Scope instances,
    // which could obviously lead to problems.
    public override int Length => Scoped.Length + 1;

    protected override Func<Scope?> CreateCurrentScopeProvider(Container container) =>
        () => GetOuterScope(Scoped.GetCurrentScope(container));

    protected override Scope? GetCurrentScopeCore(Container container) =>
        GetOuterScope(Scoped.GetCurrentScope(container));

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

    // Unfortunately, the Scope.ParentScope property is internal in Simple Injector v4.0.
    // We need to use reflection to get to it.
    private static readonly PropertyInfo ParentScopeProperty =
        typeof(Scope).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(p => p.Name == "ParentScope");

    private Scope GetParentScope(Scope? scope) => (Scope) ParentScopeProperty.GetValue(scope) ??
                                                  throw new NullReferenceException("Failed to resolve parent scope.");
}