// Module name: Mocks
// File name: RemoteServiceProviderMock.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.NullObjects;
using EyeTrackerStreaming.Shared.ServiceInterfaces;

namespace Mocks;

public class RemoteServiceProviderMock : IProvider<IRemoteService>
{
    public RemoteServiceProviderMock(Func<(bool success, IRemoteService? value)> onTryGet, Func<IRemoteService> onGet,
        Func<IObservable<IRemoteService?>> onChangesStream)
    {
        OnTryGet = onTryGet;
        OnGet = onGet;
        OnChangesStream = onChangesStream;
    }

    public static RemoteServiceProviderMock Default => new(() => (true, RemoteServiceMock.Default),
        () => RemoteServiceMock.Default, () => new NullObservable<IRemoteService?>());

    public Func<(bool success, IRemoteService? value)> OnTryGet { get; set; }
    public Func<IRemoteService> OnGet { get; set; }
    public Func<IObservable<IRemoteService?>> OnChangesStream { get; set; }

    public bool TryGet(out IRemoteService value)
    {
        var result = OnTryGet();
        value = result.value!;
        return result.success;
    }

    public IRemoteService Get()
    {
        return OnGet();
    }

    public IObservable<IRemoteService?> ChangesStream()
    {
        return OnChangesStream();
    }
}