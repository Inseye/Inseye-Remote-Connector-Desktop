// Module name: API
// File name: GrpcRemoteServiceFactory.cs
// Last edit: 2024-3-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using RemoteConnector.Proto;

namespace API.Grpc;

public class GrpcRemoteServiceFactory(
    ILogger<GrpcRemoteServiceFactory> factoryLogger,
    ILogger<IRemoteService> serviceLogger) : IRemoteServiceFactory
{
    public async ValueTask<IRemoteService> CreateRemoteService(ServiceOffer offer, CancellationToken token)
    {
        const string loggerMessage = "Creating new instance of " + nameof(GrpcRemoteService) + " for offer {offer}";
        factoryLogger.LogInformation(loggerMessage, offer);
        token.ThrowIfCancellationRequested();
        var channel = new Channel(offer.Address, offer.Port, ChannelCredentials.Insecure);
        CancellationTokenRegistration registration = default;
        try
        {
            if (token != default) registration = token.Register(() => channel.ShutdownAsync());
            serviceLogger.LogTrace("Connecting to {offer}", offer);
            try
            {
                await channel.ConnectAsync(DateTime.UtcNow.AddSeconds(5));
            }
            catch (TaskCanceledException tcs)
            {
                throw new TimeoutException($"Failed to connect to {offer.ToString()}", tcs);
            }
            return new GrpcRemoteService(new RemoteService.RemoteServiceClient(channel), offer, serviceLogger);
        }
        finally
        {
            await registration.DisposeAsync();
        }
    }
}