// Module name: EyeTrackerStreamingConsole
// File name: Program.cs
// Last edit: 2024-1-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

// See https://aka.ms/new-console-template for more information

using System.Reactive.Disposables;
using ClientCommunication.SharedMemory;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.NullObjects;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreamingConsole;
using EyeTrackerStreamingConsole.Services;
using gRPC.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Shared.DependencyInjection;
using SimpleInjector;
using TerminalGUI.DependencyInjection;
using ViewModels.DependencyInjection;


var mainCancellationTokenSource = new CancellationDisposable();
await ConsoleProgram.Run(async token =>
{
    var disposeFinishedEvent = new ManualResetEvent(false);
    try
    {
        await using var masterContainer = new Container().SetDefaultOptions();
        masterContainer
            .RegisterCrossScopeManagedService<IRemoteService, NullRemoteService>()
            .AddLogging(config =>
            {
                var serilogLogger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .WriteTo.File(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            "desktop_service.log"))
                    .CreateLogger();
                config.AddSerilog(logger: serilogLogger);
            });
        masterContainer.Verify();
        ConsoleEventHandler.SetCaptureFunction(ctrlType =>
        {
            if (ctrlType != ConsoleEventHandler.CtrlType.CTRL_CLOSE_EVENT) return false;
            mainCancellationTokenSource.Dispose();
            ConsoleEventHandler.RemoveCaptureFunction();
            disposeFinishedEvent.WaitOne();
            return true;

        });
        var parallelTasks = new[]
        {
            TerminalGuiProgram.RunUnsafe(container =>
            {
                // standard services
                container.RegisterGrpcApi();
                container.RegisterZeroconfServiceOfferProvider();
                // logging
                container.RegisterCrossContainer<ILoggerFactory>(masterContainer, Lifestyle.Singleton);
                container.Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);
                // IRemoteServiceAcctss
                container
                    .RegisterCrossContainer<IProvider<IRemoteService>>(masterContainer, Lifestyle.Scoped)
                    .RegisterCrossContainer<IPublisher<IRemoteService>>(masterContainer, Lifestyle.Scoped);
                // view models
                container.RegisterAllViewModels();
            }, token),
            ClientService(masterContainer, token)
        };
        await WrapParallelTasks(parallelTasks, () => mainCancellationTokenSource.Dispose());
    }
    finally
    {
        ConsoleEventHandler.RemoveCaptureFunction();
        disposeFinishedEvent.Set();
    }

}, mainCancellationTokenSource.Token);
return;


static async Task ClientService(Container masterContainer, CancellationToken token)
{
    try
    {
        SynchronizationContext.SetSynchronizationContext(null);
        await Task.Yield();
        var tcs = new TaskCompletionSource();
        await using var _ = token.Register(() => tcs.TrySetCanceled());
        await using var serviceContainer = new Container().SetDefaultOptions();
        serviceContainer
            .RegisterCrossContainer<ILoggerFactory>(masterContainer, Lifestyle.Singleton)
            .Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);
        
        serviceContainer
            .RegisterCrossContainer<IProvider<IRemoteService>>(masterContainer, Lifestyle.Scoped);
        serviceContainer.Register<IGazeDataSink, SharedMemoryCommunicator>(Lifestyle.Singleton);
        serviceContainer.Register<RemoteServiceToClientCommunicator>(Lifestyle.Scoped);
        serviceContainer.Verify();
        await using var scope = new Scope(serviceContainer);
        scope.GetInstance<RemoteServiceToClientCommunicator>();
        await tcs.Task;
    }
    catch (OperationCanceledException operationCanceledException)
    {
        if (operationCanceledException.CancellationToken == token)
            return;
        throw;
    }
}

static async Task WrapParallelTasks(Task[] tasks, Action onFirstFinished)
{
    List<Exception> exceptions = new List<Exception>();
    try
    {
        var finishedTask = await Task.WhenAny(tasks);
        if (finishedTask.Exception is AggregateException aggregateException)
        {
            exceptions.AddRange(aggregateException.InnerExceptions);
        }
        else if (finishedTask.Exception != null)
        {
            exceptions.Add(finishedTask.Exception);
        }
    }
    catch (Exception exception)
    {
        exceptions.Add(exception);
    }
    onFirstFinished.Invoke(); //should not throw 
    try
    {
        await Task.WhenAll(tasks.Where(t => !t.IsCompleted));
    }
    catch (AggregateException exception)
    {
        exceptions.AddRange(exception.InnerExceptions);
    }
    catch (Exception exception)
    {
        exceptions.Add(exception);
    }
    if (exceptions.Count == 0)
        return;
    throw new AggregateException(exceptions);

}