// Module name: Shared
// File name: IRouter.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

namespace EyeTrackerStreaming.Shared.Routing;

public interface IRouter
{
	public bool CanNavigateBack { get; }
	public IObservable<bool> CanNavigateBackObservable { get; }
	public Route CurrentRoute { get; }

    /// <summary>
    ///     Navigates to selected destination.
    ///     Clears navigation stack.
    /// </summary>
    /// <param name="route">Destination route</param>
    /// <param name="token">Operation cancellation token.</param>
    /// <returns></returns>
    public Task NavigateTo(Route route, CancellationToken token, object context = null);

    /// <summary>
    ///     Navigates to selected destination.
    ///     Keeps previous path on a stack and all related context.
    /// </summary>
    /// <param name="route">Destination route.</param>
    /// <param name="token">Operation cancellation token.</param>
    /// <returns></returns>
    public Task NavigateToStack(Route route, CancellationToken token, object context = null);

    /// <summary>
    ///     Navigates back to last destination pushed to stack.
    /// </summary>
    /// <param name="token">Operation cancellation token.</param>
    /// <returns></returns>
    public Task NavigateBack(CancellationToken token, object context = null);
}