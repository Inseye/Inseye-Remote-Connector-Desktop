// Module name: Shared
// File name: IRouter.cs
// Last edit: 2024-1-31 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

namespace EyeTrackerStreaming.Shared.Routing;

public interface IRouter
{
    public bool CanNavigateBack { get; }
    public IObservable<bool> CanNavigateBackObservable { get; }
    public Route CurrentRoute { get; }

    /// <summary>
    /// Navigates to selected destination.
    /// Clears navigation stack.
    /// </summary>
    /// <param name="route">Destination route</param>
    /// <param name="token">Operation cancellation token.</param>
    /// <returns></returns>
    public Task NavigateTo(Route route, CancellationToken token);

    /// <summary>
    /// Navigates to selected destination.
    /// Keeps previous path on a stack and all related context.
    /// </summary>
    /// <param name="route">Destination route.</param>
    /// <param name="token">Operation cancellation token.</param>
    /// <returns></returns>
    public Task NavigateToStack(Route route, CancellationToken token);

    /// <summary>
    /// Navigates back to last destination pushed to stack.
    /// </summary>
    /// <param name="token">Operation cancellation token.</param>
    /// <returns></returns>
    public Task NavigateBack(CancellationToken token);
}