// Module name: API
// File name: GrpcRemoteServiceFactory.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

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
                await channel.ShutdownAsync();
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