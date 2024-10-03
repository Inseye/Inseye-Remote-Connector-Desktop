// Module name: ViewModels
// File name: SearchViewModel.cs
// Last edit: 2024-07-25 09:04 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.

using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.Routing;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using EyeTrackingStreaming.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels;

public class SearchViewModel : ReactiveObject, ISearchViewModel, IDisposable
{
    private SourceList<IServiceOfferViewModel> SourceList { get; }
    public SearchViewModel(
        IPublisher<IRemoteService> publisher, IRemoteServiceFactory remoteServiceFactory,
        IRemoteServiceOffersProvider offersProvider, IRouter router, ILogger<SearchViewModel> logger)
    {
        var source = offersProvider.ServiceOffers;
        SourceList = new SourceList<IServiceOfferViewModel>(source.ToObservableChangeSet()
            .Transform(serviceOffer => (IServiceOfferViewModel) new ServiceOfferViewModel(serviceOffer, source.Count > 0 && source[^1].Equals(serviceOffer), 
                true  // TODO: Insert information about pariring here
                )));
        SourceList.DisposeWith(Disposable);
        SourceList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out _serviceOfferViewModels).Subscribe(UpdateLast).DisposeWith(Disposable);
        Logger = logger;
        RemoteServiceFactory = remoteServiceFactory;
        Publisher = publisher;
        Router = router;
        Cts = new CancellationDisposable()
            .DisposeWith(Disposable);
        ConnectTo = ReactiveCommand
            .CreateFromTask<ServiceOffer, Unit>(ConnectToHandler, CanConnectionInterationBeStarted)
            .DisposeWith(Disposable);
        CanConnectionInterationBeStarted.Value = true;
    }

    private ServiceOfferViewModel? _lastOfferViewModel = null;

    private ObservableValue<bool> CanConnectionInterationBeStarted { get; } = new(false);
    private CancellationDisposable Cts { get; }
    private CompositeDisposable Disposable { get; } = new();
    
    private IPublisher<IRemoteService> Publisher { get; }
    private IRemoteServiceFactory RemoteServiceFactory { get; }
    private IRouter Router { get; }
    private ILogger<SearchViewModel> Logger { get; }
    private readonly ReadOnlyObservableCollection<IServiceOfferViewModel> _serviceOfferViewModels;
    public ReadOnlyObservableCollection<IServiceOfferViewModel> ServiceOffers => _serviceOfferViewModels;

    public ReactiveCommand<ServiceOffer, Unit> ConnectTo { get; }

    public void Dispose()
    {
        Logger.LogTrace($"Disposing {nameof(SearchViewModel)}");
        Disposable.Dispose();
        Logger.LogTrace($"Disposed {nameof(SearchViewModel)}");
    }

    private async Task<Unit> ConnectToHandler(ServiceOffer serviceOffer)
    {
        CanConnectionInterationBeStarted.Value = false;
        try
        {
            Logger.LogTrace("ConnectToHandlerCalled");
            var service = await RemoteServiceFactory.CreateRemoteService(serviceOffer, Cts.Token);
            Logger.LogTrace("CreatedRemoteService");
            Publisher.Publish(service);
            Logger.LogTrace("PublishedRemoteService");
            await Router.NavigateTo(Route.ConnectionStatus, Cts.Token);
            Logger.LogTrace("NavigatedToConnectionStatusRoute");
        }
        catch (TimeoutException)
        {
            Logger.LogWarning("Failed to connect to service due to timeout, offer {serviceOffer}", serviceOffer);
            throw;
        }
        catch (Exception exception)
        {
            Logger.LogCritical(exception, "Failed to connect to service offer: {@serviceOffer}", serviceOffer);
            throw;
        }
        finally
        {
            CanConnectionInterationBeStarted.Value = true;
        }

        return default;
    }

    private void UpdateLast(object _)
    {
        if (ServiceOffers.Count == 0)
        {
            _lastOfferViewModel = null;
            return;
        }
        if (ServiceOffers[^1] == _lastOfferViewModel)
            return;
        if (_lastOfferViewModel != null)
            _lastOfferViewModel.IsLastItem = false;
        _lastOfferViewModel = (ServiceOfferViewModel) ServiceOffers[^1];
        _lastOfferViewModel.IsLastItem = true;

    }
    
}