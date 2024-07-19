// Module name: EyeTrackerStreamingConsole
// File name: Program.cs
// Last edit: 2024-06-18 16:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

// See https://aka.ms/new-console-template for more information

using System.Reactive.Disposables;
using ClientCommunication.NamedPipes;
using ClientCommunication.ServiceInterfaces;
using ClientCommunication.Utility;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Configuration;
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
using TerminalGUI.DependencyInjection.Extensions;
using ViewModels.DependencyInjection;
using VrChatConnector.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;


var mainCancellationTokenSource = new CancellationDisposable();
await ConsoleProgram.Run(async token =>
{
    var disposeFinishedEvent = new ManualResetEvent(false);
    ILogger? mainLogger = null;
    await using var masterContainer = new Container().SetDefaultOptions();
    try
    {
        masterContainer
            .RegisterCrossScopeManagedService<IRemoteService, NullRemoteService>();
        masterContainer.AddLogging(config =>
        {
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        "desktop_service.log"))
                .CreateLogger();
            config.AddSerilog(serilogLogger);
        });
        masterContainer.Verify();
        mainLogger = masterContainer.GetInstance<ILogger>();
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
            GuiTask(masterContainer, token),
            ClientService(masterContainer, token)
        };
        await WrapParallelTasks(parallelTasks, () => mainCancellationTokenSource.Dispose());
        mainLogger.LogInformation("Exiting main.");
    }
    catch (Exception exception)
    {
        try
        {
            mainLogger?.LogCritical(exception, "Exception occured EyeTrackerStreamingConsole main.");
        }
        catch
        {
            // ignore
        }

        throw;
    }
    finally
    {
        ConsoleEventHandler.RemoveCaptureFunction();
        disposeFinishedEvent.Set();
    }
}, mainCancellationTokenSource.Token);
return;

static Task GuiTask(Container masterContainer, CancellationToken token)
{
    return TerminalGuiProgram.RunUnsafe(container =>
    {
        // standard services
        container.RegisterGrpcApi();
        container.RegisterZeroconfServiceOfferProvider();
        // logging
        container.RegisterCrossContainer<ILoggerFactory>(masterContainer, Lifestyle.Singleton);
        container.Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);
        // circuit breaker
        container.AddLoggerCircuitBreaker();
        // IRemoteServiceAcctss
        container
            .RegisterCrossContainer<IProvider<IRemoteService>>(masterContainer, Lifestyle.Scoped)
            .RegisterCrossContainer<IPublisher<IRemoteService>>(masterContainer, Lifestyle.Scoped);
        // VRChat
        container.RegisterVrChatConnector();
        container.AddOptions(new OscClientConfiguration("127.0.0.1", 9000));
        // view models
        container.RegisterAllViewModels();
    }, token);
}

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
        serviceContainer
            .RegisterCrossScopeManagedService<IGazeDataSink, NullGazeDataSink>();
        serviceContainer.Register<RemoteServiceToClientCommunicator>(Lifestyle.Scoped);
        serviceContainer.Register<IFactory<ISharedMemoryCommunicator, string>, SharedMemoryFactory>();
        serviceContainer.RegisterDecorator<IFactory<ISharedMemoryCommunicator, string>, SharedMemoryFactoryWrapper>();
        serviceContainer.Register<NamedPipeServer>(Lifestyle.Scoped);
        serviceContainer.Verify();
        await using var scope = new Scope(serviceContainer);
        scope.GetInstance<RemoteServiceToClientCommunicator>();
        scope.GetInstance<NamedPipeServer>();
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
    var exceptions = new List<Exception>();
    try
    {
        var finishedTask = await Task.WhenAny(tasks);
        if (finishedTask.Exception is { } aggregateException)
            exceptions.AddRange(aggregateException.InnerExceptions);
        else if (finishedTask.Exception != null) exceptions.Add(finishedTask.Exception);
    }
    catch (OperationCanceledException)
    {
        // ignore
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
    catch (OperationCanceledException)
    {
        // ignore
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