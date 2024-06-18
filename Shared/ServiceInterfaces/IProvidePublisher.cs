// Module name: Shared
// File name: IProvidePublisher.cs
// Last edit: 2024-06-17 16:46 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared.ServiceInterfaces;

public interface IProvidePublisher<T> : IPublisher<T>, IProvider<T> where T : class { }