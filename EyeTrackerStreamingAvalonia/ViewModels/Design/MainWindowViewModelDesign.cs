// Module name: EyeTrackerStreamingAvalonia
// File name: MainWindowViewModel.cs
// Last edit: 2024-07-26 14:41 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using EyeTrackerStreamingAvalonia.ViewModels.Interfaces;
using ReactiveUI;

namespace EyeTrackerStreamingAvalonia.ViewModels.Design;

public class MainWindowViewModelDesign : ReactiveObject, IMainWindowViewModel
{
    public IViewModel CurrentViewModel => EmptyPageViewModel.Instance;
    public void LoadInitialView()
    {
        
    }
}