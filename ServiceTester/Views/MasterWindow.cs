// Module name: ServiceTester
// File name: MasterWindow.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Terminal.Gui;

namespace ServiceTester.Views;

public class MasterWindow : Window
{
    private readonly View _developerViews;
    private readonly View _routedView;

    public MasterWindow(GazeDataView dataView)
    {
        BorderStyle = LineStyle.None;
        _routedView = new View
        {
            Width = Dim.Percent(100),
            Height = Dim.Percent(50)
        };
        _developerViews = new View
        {
            Y = Pos.Percent(50),
            Width = Dim.Percent(100),
            Height = Dim.Percent(50),
            BorderStyle = LineStyle.None
        };
        base.Add(_routedView);
        base.Add(_developerViews);
        _developerViews.Add(dataView);
        dataView.Width = Dim.Fill();
        dataView.Height = Dim.Fill();
    }

    public override void RemoveAll()
    {
        _routedView.RemoveAll();
    }

    public override void Remove(View view)
    {
        _routedView.Remove(view);
    }

    public override void Add(View view)
    {
        _routedView.Add(view);
    }
}