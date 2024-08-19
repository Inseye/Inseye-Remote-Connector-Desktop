// Module name: Tests
// File name: CrossScopeLifetimeTests.cs
// Last edit: 2024-07-26 10:56 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.


using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Shared.DependencyInjection;
using Shared.DependencyInjection.CrossScopedObject;
using SimpleInjector;

namespace Tests;

public class CrossScopeLifetimeTests
{
    interface IInterface
    {
        public event Action OnDispose;
    }

    class ValidationClass : IInterface
    {
        public event Action? OnDispose;
    }

    class ResolvedClass : IInterface, IDisposable
    {
        public event Action? OnDispose;
        public void Dispose()
        {
            OnDispose?.Invoke();
        }
    }

    [Test]
    public void TestCrossScopeLifetimeVerify()
    {
        var container = new Container();
        container.SetDefaultOptions();
        container.RegisterCrossScopeManagedService<IInterface>();
        container.RegisterInitializer<IPublisher<IInterface>>(obj =>
        {
            if (container.IsVerifying)
                obj.Publish(new ValidationClass());
        });
        container.Verify();
    }

    [Test]
    public void TestResolveViaInterface()
    {
        var container = new Container();
        container.SetDefaultOptions();
        container.RegisterCrossScopeManagedService<IInterface>(() => new ValidationClass());
        var publisher = container.GetInstance<IPublisher<IInterface>>();
        publisher.Publish(new ResolvedClass());
        using (var scope = new Scope(container))
        {
            var @interface = scope.GetInstance<IInterface>();
        }
    }

    [Test]
    public void KeepAfterScopeDispose()
    {
        var isDisposed = false;
        var container = new Container();
        container.SetDefaultOptions();
        container.RegisterCrossScopeManagedService<IInterface>(() => new ValidationClass());
        var manager = container.GetInstance<CrossScopedObjectManager<IInterface>>();
        var resolvedClass = new ResolvedClass();
        manager.Publish(resolvedClass);
        using (manager.KeepCurrentValue())
        {
            using (var scope = new Scope(container))
            {
                var @interface = scope.GetInstance<IInterface>();
                @interface.OnDispose += () => isDisposed = true;
            }
            Assert.That(isDisposed, Is.False);
        }
        
        container.Dispose();
        Assert.That(isDisposed, Is.True);
    }

    [Test]
    public void BlockChangeUntilAllScopesAreFree()
    {
        var container = new Container();
        container.SetDefaultOptions();
        container.RegisterCrossScopeManagedService<IInterface>(() => new ValidationClass());
        var pub = container.GetInstance<IPublisher<IInterface>>();
        var resolvedClass = new ResolvedClass();
        pub.Publish(resolvedClass);
        var exception = Assert.Throws<Exception>(() =>
        {
            using (var scope = new Scope(container))
            {
                var @interface = scope.GetInstance<IInterface>();
                var publisher = container.GetInstance<IPublisher<IInterface>>();
                publisher.Publish(null);
            }
        });
        Assert.That(exception.Message, Is.EqualTo("There are users that are using current instance."));
    }
    
}