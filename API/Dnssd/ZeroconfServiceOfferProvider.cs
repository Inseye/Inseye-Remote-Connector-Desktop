// Module name: API
// File name: ZeroconfServiceProvider.cs
// Last edit: 2024-2-13 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Net;
using System.Net.Sockets;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackerStreaming.Shared.Utility;
using Microsoft.Extensions.Logging;
using Zeroconf;
using Version = EyeTrackerStreaming.Shared.Version;

namespace API.Dnssd;

public class ZeroconfServiceProvider : IRemoteServiceOffersProvider, IDisposable
{
    private DisposeBool _disposped;
    public const string Protocol = "_inseye-et._tcp.local.";
    private readonly HashSet<IZeroconfHost> _hosts = new();
    private readonly ZeroconfResolver.ResolverListener _resolverListener;
    private readonly ILogger<ZeroconfServiceProvider> _logger;
    private readonly InvokeObservable<IReadOnlyList<ServiceOffer>> _invokeObservable = new();
    public IObservable<IReadOnlyList<ServiceOffer>> ServiceOffers => !_disposped ? _invokeObservable : throw new ObjectDisposedException(nameof(ZeroconfServiceProvider));

    public ZeroconfServiceProvider(ILogger<ZeroconfServiceProvider> logger)
    {
        _logger = logger;
        _resolverListener = ZeroconfResolver.CreateListener(Protocol);
        _resolverListener.ServiceFound += ServiceFoundHandler;
        _resolverListener.ServiceLost += ServiceLostHandler;
        _resolverListener.Error += ServiceErrorHandler;
    }

    private void ServiceFoundHandler(object? _, IZeroconfHost host)
    {
        var preCount = _hosts.Count;
        _hosts.Add(host);
        if (preCount != _hosts.Count)
            PublishChanges();
    }

    private void ServiceLostHandler(object? _, IZeroconfHost host)
    {
        var preCount = _hosts.Count;
        _hosts.Remove(host);
        if (preCount != _hosts.Count)
            PublishChanges();
    }

    private void ServiceErrorHandler(object? _, Exception exception)
    {
        _invokeObservable.SendError(exception);
    }

    public void Dispose()
    {
        if(!_disposped.PerformDispose()) return;
        _resolverListener.ServiceFound -= ServiceFoundHandler;
        _resolverListener.ServiceLost -= ServiceLostHandler;
        _resolverListener.Error -= ServiceErrorHandler;
        _resolverListener.Dispose();
        _invokeObservable.Dispose();
    }

    private void PublishChanges()
    {
        IEnumerable<ServiceOffer> Selector(HashSet<IZeroconfHost> hosts)
        {
            foreach (var host in hosts)
            {
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
                        {
                            if (dict.TryGetValue("version", out var toParse))
                            {
                                try
                                {
                                    version = Version.Parse(toParse);
                                    break;
                                }
                                catch(Exception exception)
                                {
                                    _logger.LogWarning("Failed to parse remote service version {version}, exception message: {message}" ,"version", exception.Message);
                                }
                            }
                        }
                        
                        yield return new ServiceOffer(host.DisplayName, address, service.Value.Port, version);
                    }
                }
            }
        }
        _invokeObservable.Send(Selector(_hosts).ToArray());
    }
}