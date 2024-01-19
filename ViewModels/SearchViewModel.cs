// Module name: ViewModels
// File name: SearchViewModel.cs
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

using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels;

public class SearchViewModel : ReactiveObject, IDisposable
{
    private readonly CancellationDisposable _cts = new();
    private readonly CompositeDisposable _disposable = new();

    private readonly ObservableAsPropertyHelper<IReadOnlyList<ServiceOffer>> _offers;
    private readonly IPublisher<IRemoteService> _publisher;
    private readonly IRemoteServiceFactory _remoteServiceFactory;
    private readonly IRouter _router;
    private readonly ObservableAsPropertyHelper<int> _serviceUpdates;
    private readonly ILogger<SearchViewModel> _logger;

    public SearchViewModel(
        IPublisher<IRemoteService> publisher, IRemoteServiceFactory remoteServiceFactory,
        IRemoteServiceOffersProvider offersProvider, IRouter router, ILogger<SearchViewModel> logger)
    {
        _logger = logger;
        _remoteServiceFactory = remoteServiceFactory;
        _publisher = publisher;
        _router = router;
        _cts = new CancellationDisposable()
            .DisposeWith(_disposable);
        ReadOnlyObservableCollection<ServiceOffer> serviceOffers;
        _offers = offersProvider.ServiceOffers
            .Select(x => x.ToArray())
            .ToProperty(this, x => x.ServiceOffers)
            .DisposeWith(_disposable);
        _serviceUpdates = offersProvider.ServiceOffers
            .Scan(0, (prev, _) => prev + 1)
            .ToProperty(this, x => x.Updates)
            .DisposeWith(_disposable);
        ConnectTo = ReactiveCommand.CreateFromTask<ServiceOffer, Unit>(execute: ConnectToHandler)
            .DisposeWith(_disposable);
        
    }

    public IReadOnlyList<ServiceOffer> ServiceOffers => _offers.Value;
    public int Updates => _serviceUpdates.Value;

    public ReactiveCommand<ServiceOffer, Unit> ConnectTo { get; }

    public void Dispose()
    {
        _logger.LogTrace($"Disposing {nameof(SearchViewModel)}");
        _disposable.Dispose();
    }

    private async Task<Unit> ConnectToHandler(ServiceOffer serviceOffer)
    {
        _logger.LogTrace("ConnectToHandlerCalled");
        var service = await _remoteServiceFactory.CreateRemoteService(serviceOffer, _cts.Token);
        _publisher.Publish(service);
        await _router.NavigateTo(Route.ConnectionStatus, _cts.Token);
        return default;
    }
}