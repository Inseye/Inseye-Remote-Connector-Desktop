// Module name: Shared
// File name: ServiceOffer.cs
// Last edit: 2024-1-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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
        return ServiceName == other.ServiceName && Address == other.Address && Port == other.Port && Version.Equals(other.Version);
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