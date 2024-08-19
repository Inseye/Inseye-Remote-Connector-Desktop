// Module name: EyeTrackerStreamingAvalonia
// File name: CopyButton.axaml.cs
// Last edit: 2024-08-14 12:47 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.ed.


using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI;


namespace EyeTrackerStreamingAvalonia.Components;

public partial class CopyButton : Button
{
	static CopyButton()
	{
		ContentProperty.OverrideDefaultValue<CopyButton>(new Avalonia.Svg.Skia.Svg(new Uri("avres:/Assets/Svg/copy.svg")));
	}

	public static readonly StyledProperty<object?> CopiedContentProperty =
		AvaloniaProperty.Register<CopyButton, object?>(nameof(CopiedContent));

	private ICommand CopyCommand { get; } 
	public object? CopiedContent
	{
		get => GetValue(CopiedContentProperty);
		set => SetValue(CopiedContentProperty, value);
	}

	public CopyButton()
	{
		InitializeComponent();
		CopyCommand = ReactiveCommand.CreateFromTask(Copy);
	}

	protected override void OnClick()
	{
		CopyCommand.Execute(null);
		base.OnClick();
	}

	private async Task Copy()
	{
		if (CopiedContent is null)
			return;
		var topLevel = TopLevel.GetTopLevel(this);
		if (topLevel is null)
			return;
		var clipboard = topLevel.Clipboard;
		if (clipboard == null)
			return;
		switch (CopiedContent)
		{
			case IDataObject dataObject:
				await clipboard.SetDataObjectAsync(dataObject);
				break;
			case string @string:
				await clipboard.SetTextAsync(@string);
				break;
			default:
				await clipboard.SetTextAsync(CopiedContent.ToString());
				break;
		}
		
	}
}