// Module name: Mocks
// File name: RemoteServiceOfferProviderMock.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using Version = EyeTrackerStreaming.Shared.Version;

namespace Mocks;

public class RemoteServiceOffersProviderMock : IRemoteServiceOffersProvider, IDisposable
{
    private readonly Task _backgroundTask;
    private readonly InvokeObservable<IReadOnlyList<ServiceOffer>> _invokeObservable = new();
    private readonly CancellationTokenSource _tcs = new();

    public RemoteServiceOffersProviderMock()
    {
        (_backgroundTask = BackgroundTask()).ConfigureAwait(false);
    }


    public void Dispose()
    {
        _tcs.Cancel();
        _tcs.Dispose();
        _backgroundTask.Wait();
    }

    public IObservable<IReadOnlyList<ServiceOffer>> ServiceOffers => _invokeObservable;

    private async Task BackgroundTask()
    {
        var random = new Random();
        try
        {
            while (!_tcs.IsCancellationRequested)
            {
                _invokeObservable.Send(Enumerable.Range(0, (int) random.NextInt64(1, 5))
                    .Select(i => new ServiceOffer($"Service Name{i}", "address", 1234, new Version(0, 0, 0, "mock")))
                    .ToList());
                await Task.Delay(3000).ConfigureAwait(false);
            }
        }
        catch (TaskCanceledException taskCanceledException)
        {
            // ignore
        }
        catch (Exception exception)
        {
            _invokeObservable.SendError(exception);
        }
        finally
        {
            _invokeObservable.Complete();
        }
    }
}