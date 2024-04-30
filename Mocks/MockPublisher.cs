// Module name: Mocks
// File name: MockPublisher.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.ServiceInterfaces;

namespace Mocks;

public class MockPublisher<T> : IPublisher<T> where T : class
{
    public Action<T?> OnPublish { get; set; } = _ => { };

    public void Publish(T? value)
    {
        OnPublish(value);
    }
}