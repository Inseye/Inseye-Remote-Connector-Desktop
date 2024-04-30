// Module name: Tests
// File name: TestStatusViewModel.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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