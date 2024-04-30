// Module name: TerminalGUI
// File name: DisposingView.cs
// Last edit: 2024-04-30 12:22 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Reactive.Disposables;
using ReactiveUI;
using Terminal.Gui;

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

    public T ViewModel => _viewModel;

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

    protected sealed override void Dispose(bool disposing)
    {
        Disposable.Dispose();
        base.Dispose(disposing);
    }
}