// Module name: Tests
// File name: TestGrpcRemoteService.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using API.Grpc;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Mocks.API;
using Mocks.Extensions;
using RemoteConnector.Proto;
using Version = EyeTrackerStreaming.Shared.Version;

namespace Tests.APITests;

public class TestGrpcRemoteService
{
    [Test]
    public void TestRemoteServiceStatusOnRemoteServiceStatusFinished()
    {
        var @event = new AutoResetEvent(false);
        IEnumerable<EyeTrackerAvailability> StatusSource(AutoResetEvent ev)
        {
            yield return new EyeTrackerAvailability
            {
                Status = EyeTrackerAvailability.Types.Status.Available
            };
            @event.WaitOne();
            yield return new EyeTrackerAvailability
            {
                Status = EyeTrackerAvailability.Types.Status.Available
            };
            
        }

        var service = new GrpcRemoteService(new ApiGrpcRemoteClientMock
        {
            OnObserveEyeTrackerAvailability = (_, opt) =>
                StatusSource(@event).ToAsyncServerStreamingCall(null, opt.CancellationToken)
        }, new ServiceOffer("mock", "0.0.0.0", 0, new Version(0, 0, 1)), NullLogger<IRemoteService>.Instance);
        Assert.That(service.ServiceStatus, Is.EqualTo(RemoteServiceStatus.Connected));
        @event.Set();
        Thread.Sleep(100); // wait for the iterator to expire
        Assert.That(service.ServiceStatus, Is.EqualTo(RemoteServiceStatus.Disconnected));
    }
}