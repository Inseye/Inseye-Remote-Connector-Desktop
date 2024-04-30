// Module name: API
// File name: ZeroconfServiceOfferProvider.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Net;
using System.Net.Sockets;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Structs;
using EyeTrackerStreaming.Shared.Utility;
using Microsoft.Extensions.Logging;
using Zeroconf;
using Version = EyeTrackerStreaming.Shared.Version;

namespace API.Dnssd;

public class ZeroconfServiceProvider : IRemoteServiceOffersProvider, IDisposable
{
    public const string Protocol = "_inseye-et._tcp.local.";
    private DisposeBool _disposped;

    public ZeroconfServiceProvider(ILogger<ZeroconfServiceProvider> logger)
    {
        Logger = logger;
        SynchronizationContextExtensions.RunOnNull(() => ZeroconfLoop(CancellationTokenSource.Token));
    }

    private HashSet<ServiceOffer> Offers { get; set; } = new();
    private ILogger<ZeroconfServiceProvider> Logger { get; }
    private CancellationTokenSource CancellationTokenSource { get; } = new();
    private InvokeObservable<IReadOnlyList<ServiceOffer>> InvokeServiceOffersObservable { get; } = new();

    public void Dispose()
    {
        if (!_disposped.PerformDispose()) return;
        CancellationTokenSource.Cancel();
        CancellationTokenSource.Dispose();
        InvokeServiceOffersObservable.Dispose();
    }

    public IObservable<IReadOnlyList<ServiceOffer>> ServiceOffers => !_disposped
        ? InvokeServiceOffersObservable
        : throw new ObjectDisposedException(nameof(ZeroconfServiceProvider));

    private async Task ZeroconfLoop(CancellationToken token)
    {
        HashSet<ServiceOffer> newSet = new();
        while (!token.IsCancellationRequested)
            try
            {
                var hosts = await ZeroconfResolver.ResolveAsync(Protocol, cancellationToken: token);
                newSet.Clear();
                foreach (var host in hosts)
                foreach (var offer in ToServiceOffers(host))
                    newSet.Add(offer);
                if (newSet.SetEquals(Offers))
                    continue;
                (Offers, newSet) = (newSet, Offers);
                PublishChanges();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Error encountered in ZeroconfLoop");
            }

        Logger.LogTrace("Terminating ZeroconfLoop");
    }

    private void PublishChanges()
    {
        InvokeServiceOffersObservable.Send(Offers.ToArray());
    }

    private IEnumerable<ServiceOffer> ToServiceOffers(IZeroconfHost host)
    {
        if (host.IPAddresses is null)
            yield break;
        foreach (var address in host.IPAddresses)
        {
            if (!IPAddress.TryParse(address, out var addres))
                continue;
            if (addres.AddressFamily != AddressFamily.InterNetwork)
                continue;
            foreach (var service in host.Services)
            {
                var version = new Version(0, 0, 0, "unknown");
                foreach (var dict in service.Value.Properties)
                    if (dict.TryGetValue("version", out var toParse))
                        try
                        {
                            version = Version.Parse(toParse);
                            break;
                        }
                        catch (Exception exception)
                        {
                            Logger.LogWarning(
                                "Failed to parse remote service version {version}, exception message: {message}",
                                "version", exception.Message);
                        }

                yield return new ServiceOffer(host.DisplayName, address, service.Value.Port, version);
            }
        }
    }
}