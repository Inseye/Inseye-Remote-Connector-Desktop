// Module name: TerminalGUI
// File name: DisposingView.cs
// Last edit: 2024-1-31 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
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

using System.Reactive.Disposables;
using ReactiveUI;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace TerminalGUI;

public class DisposingView<T> : View, IViewFor<T>
    where T : class
{
    protected readonly CompositeDisposable Disposable = new();
    private T _viewModel;

    public DisposingView(T viewModel) : base()
    {
        _viewModel = viewModel;
        Width = Dim.Fill();
        Height = Dim.Fill();
        BorderStyle = LineStyle.Single;
    }

    object? IViewFor.ViewModel
    {
        get => _viewModel;
        set => _viewModel = (T) value!;
    }

    T? IViewFor<T>.ViewModel
    {
        get => _viewModel;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(ViewModel));
            _viewModel = value;
        }
    }

    public T ViewModel => _viewModel;

    protected sealed override void Dispose(bool disposing)
    {
        Disposable.Dispose();
        base.Dispose(disposing);
    }
}