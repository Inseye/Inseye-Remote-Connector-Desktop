// Module name: ClientCommunication
// File name: TestHelper.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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