// Module name: Mocks
// File name: RemoteServiceOfferProviderMock.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections.ObjectModel;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Version = EyeTrackerStreaming.Shared.Version;

namespace Mocks;

public class RemoteServiceOffersProviderMock : IRemoteServiceOffersProvider, IDisposable
{
    private readonly Task _backgroundTask;
    private readonly ObservableCollection<ServiceOffer> _invokeObservable = new();
    private readonly CancellationTokenSource _tcs = new();

    public RemoteServiceOffersProviderMock()
    {
        (_backgroundTask = BackgroundTask()).ConfigureAwait(false);
        ServiceOffers = new ReadOnlyObservableCollection<ServiceOffer>(_invokeObservable);
    }


    public void Dispose()
    {
        _tcs.Cancel();
        _tcs.Dispose();
        _backgroundTask.Wait();
    }

    public ReadOnlyObservableCollection<ServiceOffer> ServiceOffers { get; } 

    private async Task BackgroundTask()
    {
        var random = new Random();
        try
        {
            while (!_tcs.IsCancellationRequested)
            {
                _invokeObservable.Clear();       
                foreach (var offer in Enumerable.Range(0, (int) random.NextInt64(1, 5))
                             .Select(i => new ServiceOffer($"Service Name{i}", "address", 1234, new Version(0, 0, 0, "mock"))))
                {
                    _invokeObservable.Add(offer);
                }
                await Task.Delay(3000).ConfigureAwait(false);
            }
        }
        catch (TaskCanceledException taskCanceledException)
        {
            // ignore
        }
    }
}