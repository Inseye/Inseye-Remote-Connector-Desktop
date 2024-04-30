// Module name: Shared
// File name: IUserAuthorizationRequestHandler.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Authorization;

namespace EyeTrackerStreaming.Shared.ServiceInterfaces;

/// <summary>
///     Interface of service when needed is able to ask user if they authorize specified client application
/// </summary>
public interface IUserAuthorizationRequestHandler
{
    Task<bool> PromptUserForAuthorization(AuthorizedClient client, CancellationToken token);
}