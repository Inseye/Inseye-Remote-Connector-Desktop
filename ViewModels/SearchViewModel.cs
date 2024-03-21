// Module name: ViewModels
// File name: SearchViewModel.cs
// Last edit: 2024-3-21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels;

public class SearchViewModel : ReactiveObject, IDisposable
{
    public SearchViewModel(
        IPublisher<IRemoteService> publisher, IRemoteServiceFactory remoteServiceFactory,
        IRemoteServiceOffersProvider offersProvider, IRouter router, ILogger<SearchViewModel> logger)
    {
        Logger = logger;
        RemoteServiceFactory = remoteServiceFactory;
        Publisher = publisher;
        Router = router;
        Cts = new CancellationDisposable()
            .DisposeWith(Disposable);
        ReadOnlyObservableCollection<ServiceOffer> serviceOffers;
        Offers = offersProvider.ServiceOffers
            .Select(x => x.ToArray())
            .ToProperty(this, x => x.ServiceOffers)
            .DisposeWith(Disposable);
        ServiceUpdates = offersProvider.ServiceOffers
            .Scan(0, (prev, _) => prev + 1)
            .ToProperty(this, x => x.Updates)
            .DisposeWith(Disposable);
        ConnectTo = ReactiveCommand
            .CreateFromTask<ServiceOffer, Unit>(
                arg => SynchronizationContextExtensions.RunOnNull(ConnectToHandler, arg))
            .DisposeWith(Disposable);
    }

    private CancellationDisposable Cts { get; }
    private CompositeDisposable Disposable { get; } = new();

    private ObservableAsPropertyHelper<IReadOnlyList<ServiceOffer>> Offers { get; }
    private IPublisher<IRemoteService> Publisher { get; }
    private IRemoteServiceFactory RemoteServiceFactory { get; }
    private IRouter Router { get; }
    private ObservableAsPropertyHelper<int> ServiceUpdates { get; }
    private ILogger<SearchViewModel> Logger { get; }

    public IReadOnlyList<ServiceOffer> ServiceOffers => Offers.Value;
    public int Updates => ServiceUpdates.Value;

    public ReactiveCommand<ServiceOffer, Unit> ConnectTo { get; }

    public void Dispose()
    {
        Logger.LogTrace($"Disposing {nameof(SearchViewModel)}");
        Disposable.Dispose();
    }

    private async Task<Unit> ConnectToHandler(ServiceOffer serviceOffer)
    {
        try
        {
            Logger.LogTrace("ConnectToHandlerCalled");
            var service = await RemoteServiceFactory.CreateRemoteService(serviceOffer, Cts.Token);
            Publisher.Publish(service);
            await Router.NavigateTo(Route.ConnectionStatus, Cts.Token);
        }
        catch (Exception exception)
        {
            Logger.LogCritical(exception, "Failed to connect to service offer: {@serviceOffer}", serviceOffer);
        }

        return default;
    }
}