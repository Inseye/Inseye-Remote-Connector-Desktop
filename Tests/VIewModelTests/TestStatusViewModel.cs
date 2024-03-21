// Module name: Tests
// File name: TestStatusViewModel.cs
// Last edit: 2024-3-21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackingStreaming.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Reactive.Testing;
using Mocks;
using ReactiveUI;

namespace Tests.VIewModelTests;

public class TestStatusViewModel
{
    [Test]
    public void TestStatusViewThrowOnCalibrationStartException()
    {
        Assert.DoesNotThrow(() =>
        {
            var testScheduler = new TestScheduler();
            RxApp.MainThreadScheduler = testScheduler;
            var viewModel = new StatusViewModel(RemoteServiceProviderMock.Default, new CalibrationHandlerMock
                {
                    OnCalibrationHandler = (_, _) => throw new Exception("Exception on call")
                },
                NullLogger<StatusViewModel>.Instance, new RouterMock(), new MockPublisher<IRemoteService>());
            viewModel.BeginCalibration.Execute();
            testScheduler.AdvanceBy(1_000);
        });
    }
}