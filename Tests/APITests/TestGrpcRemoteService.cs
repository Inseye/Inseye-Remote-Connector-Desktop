// Module name: Tests
// File name: TestGrpcRemoteService.cs
// Last edit: 2024-3-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

        static IEnumerable<EyeTrackerAvailability> StatusSource(AutoResetEvent ev)
        {
            yield return new EyeTrackerAvailability
            {
                Status = EyeTrackerAvailability.Types.Status.Available
            };
            yield return new EyeTrackerAvailability
            {
                Status = EyeTrackerAvailability.Types.Status.Available
            };
            ev.Set();
        }

        var service = new GrpcRemoteService(new ApiGrpcRemoteClientMock
        {
            OnObserveEyeTrackerAvailability = (_, opt) =>
                StatusSource(@event).ToAsyncServerStreamingCall(null, opt.CancellationToken)
        }, new ServiceOffer("mock", "0.0.0.0", 0, new Version(0, 0, 1)), NullLogger<IRemoteService>.Instance);
        Assert.That(service.ServiceStatus, Is.EqualTo(RemoteServiceStatus.Connected));
        @event.WaitOne();
        Assert.That(service.ServiceStatus, Is.EqualTo(RemoteServiceStatus.Disconnected));
    }
}