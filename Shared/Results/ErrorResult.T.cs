// Module name: Shared
// File name: ErrorResult.T.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Results.Interfaces;

namespace EyeTrackerStreaming.Shared.Results;

public class ErrorResult<T> : Result<T>, IErrorResult
{
    public ErrorResult(string errorMessage) : base(default!)
    {
        Success = false;
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; protected set; }
}