// Module name: EyeTrackerStreamingConsole
// File name: ConsoleEventHandler.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Runtime.InteropServices;

namespace EyeTrackerStreamingConsole;

/// <summary>
///     Helper class that allows capturing Windows native window console events.
/// </summary>
public static class ConsoleEventHandler
{
    public delegate bool EventHandler(CtrlType sig);

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

    private static readonly object Lock = new();
    private static bool _hasSetHandler = false;
    private static EventHandler? _dynamicHandler;
    private static readonly EventHandler Handler = StaticHandler;

    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

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