// Module name: Mocks
// File name: ConstOption.cs
// Last edit: 2024-06-18 11:01 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Microsoft.Extensions.Options;

namespace EyeTrackerStreaming.Shared.Configuration;

public record ConstOption<T>(T Value) : IOptions<T> where T : class;