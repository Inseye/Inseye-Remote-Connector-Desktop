// Module name: Mocks
// File name: RemoteServiceFactoryMock.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;

namespace Mocks;

public class RemoteServiceFactoryMock : IRemoteServiceFactory
{
    public Func<ServiceOffer, CancellationToken, ValueTask<IRemoteService>> OnCreateRemoteService { get; set; } =
        (offer, _) => new ValueTask<IRemoteService>(new RemoteServiceMock(offer));

    public ValueTask<IRemoteService> CreateRemoteService(ServiceOffer offer, CancellationToken token)
    {
        return OnCreateRemoteService(offer, token);
    }
}