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
using SimpleInjector;

namespace Tests;

public class CrossScopeLifetimeTests
{
    interface IInterface
    {
        public event Action OnDispose;
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
                obj.Publish(new ResolvedClass());
        });
        container.Verify();
    }

    [Test]
    public void TestResolveViaInterface()
    {
        var container = new Container();
        container.SetDefaultOptions();
        container.RegisterCrossScopeManagedService<IInterface>(() => new ResolvedClass());
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
        container.RegisterCrossScopeManagedService<IInterface>(() => new ResolvedClass());
        using (var scope = new Scope(container))
        {
            var @interface = scope.GetInstance<IInterface>();
            @interface.OnDispose += () => isDisposed = true;
        }
        Assert.That(isDisposed, Is.False);
        container.Dispose();
        Assert.That(isDisposed, Is.True);
    }

    [Test]
    public void BlockChangeUntilAllScopesAreFree()
    {
        var container = new Container();
        container.SetDefaultOptions();
        container.RegisterCrossScopeManagedService<IInterface>(() => new ResolvedClass());
        using (var scope = new Scope(container))
        {
            var @interface = scope.GetInstance<IInterface>();
            var publisher = container.GetInstance<IPublisher<IInterface>>();
            publisher.Publish(null);
        }
    }
    
}