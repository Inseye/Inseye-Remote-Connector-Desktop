// Module name: Tests
// File name: NullPublisher.cs
// Last edit: 2024-07-30 15:15 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.ServiceInterfaces;

namespace Tests.Mocks;

public class NullPublisher<T> : IPublisher<T> where T : class
{
    public static NullPublisher<T> Instance { get; } = new();
    private NullPublisher() {}
    public void Publish(T? value)
    {
        
    }
}