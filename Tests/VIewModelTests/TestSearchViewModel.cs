// Module name: Tests
// File name: TestSearchViewModel.cs
// Last edit: 2024-07-30 15:12 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections.ObjectModel;
using EyeTrackerStreaming.Shared;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using EyeTrackingStreaming.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Mocks;
using Tests.Mocks;
using Version = EyeTrackerStreaming.Shared.Version;

namespace Tests.VIewModelTests;

public class TestSearchViewModel
{
    private class InvokeOffersProvider : IRemoteServiceOffersProvider
    {
        public InvokeOffersProvider(ObservableCollection<ServiceOffer> observable)
        {
            ServiceOffers = new ReadOnlyObservableCollection<ServiceOffer>(observable);
        }
        
        public ReadOnlyObservableCollection<ServiceOffer> ServiceOffers { get; }
    }
    
    [Test]
    public void TestViewModelServiceOffers()
    {
        var sourceCollection = new ObservableCollection<ServiceOffer>();
        sourceCollection.Add(new ServiceOffer("test 1", "127.0.0.1", 66, new Version(0, 0, 1)));
        sourceCollection.Add(new ServiceOffer("test 2", "127.0.0.2", 104, new Version(0, 0, 1)));
        sourceCollection.Add(new ServiceOffer("test 3", "127.0.0.3", 206, new Version(0, 0, 1)));
        var searchViewModel = new SearchViewModel(NullPublisher<IRemoteService>.Instance,
            new RemoteServiceFactoryMock(), new InvokeOffersProvider(sourceCollection), new RouterMock(),
            new NullLogger<SearchViewModel>());
        Assert.That(searchViewModel.ServiceOffers.Count, Is.EqualTo(3));
        var last = searchViewModel.ServiceOffers[^1];
        sourceCollection.RemoveAt(1);
        Assert.That(searchViewModel.ServiceOffers.Count, Is.EqualTo(2));
        Assert.That(searchViewModel.ServiceOffers[^1], Is.EqualTo(last));
        sourceCollection.Add(new ServiceOffer("test 4", "127.0.0,4", 1234, new Version(0, 0, 1)));
        Assert.That(searchViewModel.ServiceOffers.Count, Is.EqualTo(3));
    }
}