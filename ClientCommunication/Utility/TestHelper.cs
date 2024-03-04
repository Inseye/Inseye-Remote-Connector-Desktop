﻿// Module name: ClientCommunication
// File name: TestHelper.cs
// Last edit: 2024-2-29 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Runtime.InteropServices;
using ClientCommunication.NamedPipes.Messages.Memory;

namespace ClientCommunication.Utility;

public static class TestHelper
{
    private const int ExpectedMemoryLayoutSize = 1024;
    public static void AssertMemoryLayoutStructSize()
    {
        var actualSize = Marshal.SizeOf<ServiceInfoResponseMemoryLayout>();
        if (actualSize != ExpectedMemoryLayoutSize)
            throw new Exception(
                $"Invalid memory size of {nameof(ServiceInfoResponseMemoryLayout)}, expected {ExpectedMemoryLayoutSize} but got {actualSize} instead.");
    }
    
}