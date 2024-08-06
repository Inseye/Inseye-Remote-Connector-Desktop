// Module name: ViewModels
// File name: OfferViewModel.cs
// Last edit: 2024-07-30 12:51 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreaming.Shared;
using EyeTrackingStreaming.ViewModels.Interfaces;
using ReactiveUI;

namespace EyeTrackingStreaming.ViewModels;

public class ServiceOfferViewModel : ReactiveObject, IServiceOfferViewModel
{
    private bool _isLastItem;
    private string _deviceName = string.Empty;
    private string _ipAddressWithPort = string.Empty;
    private string _protocolVersion = string.Empty;
    private ServiceOffer _serviceOffer;
    private bool _isPaired;

    public ServiceOfferViewModel(ServiceOffer model, bool isLastItem, bool isPaired)
    {
        ServiceOffer = model;
        IsLastItem = isLastItem;
        _isPaired = isPaired;
    }

    public ServiceOffer ServiceOffer
    {
        get => _serviceOffer;
        set
        {
            if(EqualityComparer<ServiceOffer>.Default.Equals(_serviceOffer, value))
                return;
            _serviceOffer = value;
            DeviceName = _serviceOffer.ServiceName;
            IpAddressWithPort = $"{_serviceOffer.Address}:{_serviceOffer.Port}";
            ProtocolVersion = _serviceOffer.Version.ToString();
        } 
    }
    

    public string ProtocolVersion
    {
        get => _protocolVersion;
        private set => this.RaiseAndSetIfChanged(ref _protocolVersion, value);
    }
    public string DeviceName
    {
        get => _deviceName;
        private set => this.RaiseAndSetIfChanged(ref _deviceName, value);
    }
    
    public bool IsLastItem
    {
        get => _isLastItem;
        set => this.RaiseAndSetIfChanged(ref _isLastItem, value);
    }

    public string IpAddressWithPort
    {
        get => _ipAddressWithPort;
        private set => this.RaiseAndSetIfChanged(ref _ipAddressWithPort, value);
    }

    public bool IsPaired
    {
        get => _isPaired;
        set => this.RaiseAndSetIfChanged(ref _isPaired, value);
    }
}

