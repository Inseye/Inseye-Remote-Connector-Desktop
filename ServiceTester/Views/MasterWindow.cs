// Module name: ServiceTester
// File name: MasterWindow.cs
// Last edit: 2024-2-15 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using Terminal.Gui;

namespace ServiceTester.Views;

public class MasterWindow : Window
{
    private readonly View _routedView;
    private readonly View _developerViews;

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