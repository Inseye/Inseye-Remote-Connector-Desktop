// Module name: ViewModels
// File name: SearchViewModel.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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
                arg => SynchronizationContextExtensions.RunOnNull(ConnectToHandler, arg),
                CanConnectionInterationBeStarted)
            .DisposeWith(Disposable);
        CanConnectionInterationBeStarted.Value = true;
    }

    private ObservableValue<bool> CanConnectionInterationBeStarted { get; } = new(false);
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
        CanConnectionInterationBeStarted.Value = false;
        try
        {
            Logger.LogTrace("ConnectToHandlerCalled");
            var service = await RemoteServiceFactory.CreateRemoteService(serviceOffer, Cts.Token);
            Publisher.Publish(service);
            await Router.NavigateTo(Route.ConnectionStatus, Cts.Token);
        }
        catch (TimeoutException)
        {
            Logger.LogWarning("Failed to connect to service due to timeout, offer {serviceOffer}", serviceOffer);
            throw;
        }
        catch (Exception exception)
        {
            Logger.LogCritical(exception, "Failed to connect to service offer: {@serviceOffer}", serviceOffer);
        }
        finally
        {
            CanConnectionInterationBeStarted.Value = true;
        }

        return default;
    }
}