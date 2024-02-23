// Module name: EyeTrackerStreamingConsole
// File name: ConsoleEventCapturer.cs
// Last edit: 2024-2-21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

namespace EyeTrackerStreamingConsole;
/// <summary>
/// Helper class that allows capturing Windows native window console events.
/// </summary>
public static class ConsoleEventHandler
{
    private static readonly object Lock = new();
    private static bool _hasSetHandler = false;
    private static EventHandler? _dynamicHandler;
     
    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

    public delegate bool EventHandler(CtrlType sig);
    private static readonly EventHandler Handler = StaticHandler;

    public enum CtrlType
    {
        // ReSharper disable InconsistentNaming
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6
        // ReSharper restore InconsistentNaming
    }

    private static bool StaticHandler(CtrlType sig)
    {
        return _dynamicHandler!(sig);
    }

    public static void SetCaptureFunction(EventHandler func)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));
        lock (Lock)
        {
            _dynamicHandler = func;
            if (_hasSetHandler) return;
            if (!SetConsoleCtrlHandler(StaticHandler, true))
                throw new Exception("Failed to add handler");
            _hasSetHandler = true;
        }
    }

    public static void RemoveCaptureFunction()
    {
        // ReSharper disable once InconsistentlySynchronizedField
        if (!_hasSetHandler)
            return;
        lock (Lock)
        {
            if (!_hasSetHandler)
                return;
            SetConsoleCtrlHandler(StaticHandler, false); // TODO: Maybe check return value
            _dynamicHandler = null;
            _hasSetHandler = false;
        }
    }
    
    
}