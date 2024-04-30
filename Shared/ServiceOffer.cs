// Module name: Shared
// File name: ServiceOffer.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace EyeTrackerStreaming.Shared;

[Serializable]
public readonly struct ServiceOffer : IEquatable<ServiceOffer>
{
    public readonly string ServiceName;
    public readonly string Address;
    public readonly int Port;
    public readonly Version Version;

    public ServiceOffer(string serviceName, string address, int port, Version version)
    {
        ServiceName = serviceName;
        Address = address;
        Port = port;
        Version = version;
    }

    public override string ToString()
    {
        return $"{ServiceName} at {Address}:{Port} ver: {Version}";
    }

    public bool Equals(ServiceOffer other)
    {
        return ServiceName == other.ServiceName && Address == other.Address && Port == other.Port &&
               Version.Equals(other.Version);
    }

    public override bool Equals(object? obj)
    {
        return obj is ServiceOffer other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ServiceName, Address, Port, Version);
    }
}