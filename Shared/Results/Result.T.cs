﻿// Module name: Shared
// File name: Result.T.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared.Results;

public abstract class Result<T> : Result
{
    private T _data;

    public Result(T data)
    {
        Success = true;
        _data = data;
    }

    public T Data
    {
        get => Success
            ? _data
            : throw new Exception($"You can't access .{nameof(Data)} when .{nameof(Success)} is false");
        set => _data = value;
    }
}