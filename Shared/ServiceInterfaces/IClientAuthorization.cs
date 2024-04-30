// Module name: Shared
// File name: IClientAuthorization.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Authorization;

namespace EyeTrackerStreaming.Shared.ServiceInterfaces;

/// <summary>
///     Interface for service that authorizes client desktop application
/// </summary>
public interface IClientAuthorization
{
    bool IsClientAuthorized(AuthorizedClient client);
    Task<bool> AuthorizeClient(AuthorizedClient client, CancellationToken token);
}