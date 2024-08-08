// Module name: Shared.DependencyInjection
// File name: CrossScopedLifetime.cs
// Last edit: 2024-07-26 10:00 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.


using System.Linq.Expressions;
using SimpleInjector;

namespace Shared.DependencyInjection.CrossScopedObject;

internal class CrossScopedLifetime : Lifestyle
{
    public static CrossScopedLifetime Instance = new();
    private CrossScopedLifetime() : base(nameof(CrossScopedLifetime)) { }

    protected override Registration CreateRegistrationCore(Type concreteType, Container container)
    {
        return new CrossScopedRegistration(container, concreteType);
    }

    protected override Registration CreateRegistrationCore<TService>(Func<TService> instanceCreator, Container container)
    {
        return new CrossScopedRegistration(container, typeof(TService));
    }
    
    public override int Length { get; } = 600;

    private class CrossScopedRegistration : Registration
    {
        public CrossScopedRegistration(Container container, Type implementationType) : base(Instance, container, implementationType)
        {
            
        }

        public override Expression BuildExpression()
        {
            return Expression.Call(Expression.Constant(this), GetType().GetMethod(nameof(GetInstance))!.MakeGenericMethod(ImplementationType));
        }
        
        public TImplementation GetInstance<TImplementation>() where TImplementation : class
        {
            var currentScope = Scoped.GetCurrentScope(Container);
            if (currentScope == null)
                throw new Exception("Scope is null");
            var implementation = Container.GetInstance<CrossScopedObjectManager<TImplementation>>();
            var instance =  implementation.Get();
            if (instance == null)
                return instance!; // this problem will be handled by Container
            // currentScope.WhenScopeEnds(WhenScopeEndsHandler<TImplementation>);
            currentScope.RegisterForDisposal(new Disposable<TImplementation>(Container));
            implementation.IncrementCounter();
            return instance;
        }

        private void WhenScopeEndsHandler<TImplementation>() where TImplementation : class
        {
            var implementation = Container.GetInstance<CrossScopedObjectManager<TImplementation>>();
            implementation.DecrementCounter();
        }

        private class Disposable<TImplementation> : IDisposable where TImplementation : class
        {
            private readonly Container _container;

            public Disposable(Container container)
            {
                _container = container;
            }
            
            public void Dispose()
            {
                var implementation = _container.GetInstance<CrossScopedObjectManager<TImplementation>>();
                implementation.DecrementCounter();
            }
        }
    }
}