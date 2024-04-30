// Module name: TerminalGUI
// File name: TerminalUiUserAuthorizationRequestHandler.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared.Authorization;
using EyeTrackerStreaming.Shared.ServiceInterfaces;

namespace TerminalGUI.Views;

public class TerminalUiUserAuthorizationRequestHandler : IUserAuthorizationRequestHandler
{
    public TerminalUiUserAuthorizationRequestHandler(TerminalGuiRouter terminalGuiRouter)
    {
    }

    public Task<bool> PromptUserForAuthorization(AuthorizedClient client, CancellationToken token)
    {
        return Task.FromResult(true);
    }
}