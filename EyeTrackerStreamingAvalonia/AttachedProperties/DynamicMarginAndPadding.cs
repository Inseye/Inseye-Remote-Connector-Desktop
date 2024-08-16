// Module name: EyeTrackerStreamingAvalonia
// File name: DynamicMargin.cs
// Last edit: 2024-07-29 10:13 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Avalonia;
using Avalonia.Controls;


namespace EyeTrackerStreamingAvalonia.AttachedProperties;

public class DynamicMarginAndPadding
{
	public static readonly AttachedProperty<double> LeftMarginProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Control, double>("LeftMargin", double.NaN);

	public static readonly AttachedProperty<double> TopMarginProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Control, double>("TopMargin", double.NaN);

	public static readonly AttachedProperty<double> RightMarginProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Control, double>("RightMargin", double.NaN);

	public static readonly AttachedProperty<double> BottomMarginProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Control, double>("BottomMargin", double.NaN);

	public static readonly AttachedProperty<double> HorizontalMarginProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Control, double>("HorizontalMargin", double.NaN);

	public static readonly AttachedProperty<double> VerticalMarginProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Control, double>("VerticalMargin", double.NaN);

	public static readonly AttachedProperty<double> LeftPaddingProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Decorator, double>("LeftPadding", double.NaN);

	public static readonly AttachedProperty<double> TopPaddingProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Decorator, double>("TopPadding", double.NaN);

	public static readonly AttachedProperty<double> RightPaddingProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Decorator, double>("RightPadding", double.NaN);

	public static readonly AttachedProperty<double> BottomPaddingProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Decorator, double>("BottomPadding", double.NaN);

	public static readonly AttachedProperty<double> HorizontalPaddingProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Decorator, double>("HorizontalPadding", double.NaN);

	public static readonly AttachedProperty<double> VerticalPaddingProperty =
		AvaloniaProperty.RegisterAttached<DynamicMarginAndPadding, Decorator, double>("VerticalPadding", double.NaN);

	static DynamicMarginAndPadding()
	{
		LeftPaddingProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Decorator decorator || double.IsNaN(value))
				return;
			var prev = decorator.Padding;
			decorator.Padding = new Thickness(value, prev.Top, prev.Right, prev.Bottom);
		});
		TopPaddingProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Decorator decorator || double.IsNaN(value))
				return;
			var prev = decorator.Padding;
			decorator.Padding = new Thickness(prev.Left, value, prev.Right, prev.Bottom);
		});
		RightPaddingProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Decorator decorator || double.IsNaN(value))
				return;
			var prev = decorator.Padding;
			decorator.Padding = new Thickness(prev.Left, prev.Top, value, prev.Bottom);
		});
		BottomPaddingProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Decorator decorator || double.IsNaN(value))
				return;
			var prev = decorator.Padding;
			decorator.Padding = new Thickness(prev.Left, prev.Top, prev.Right, value);
		});
		VerticalPaddingProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Decorator decorator || double.IsNaN(value))
				return;
			var prev = decorator.Padding;
			decorator.Padding = new Thickness(prev.Left, value, prev.Right, value);
		});
		HorizontalPaddingProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Decorator decorator || double.IsNaN(value))
				return;
			var prev = decorator.Padding;
			decorator.Padding = new Thickness(value, prev.Top, value, prev.Bottom);
		});
		LeftMarginProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Control control || double.IsNaN(value))
				return;
			var prev = control.Margin;
			control.Margin = new Thickness(value, prev.Top, prev.Right, prev.Bottom);
		});
		TopMarginProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Control control || double.IsNaN(value))
				return;
			var prev = control.Margin;
			control.Margin = new Thickness(prev.Left, value, prev.Right, prev.Bottom);
		});

		RightMarginProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Control control || double.IsNaN(value))
				return;
			var prev = control.Margin;
			control.Margin = new Thickness(prev.Left, prev.Top, value, prev.Bottom);
		});
		BottomMarginProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Control control || double.IsNaN(value))
				return;
			var prev = control.Margin;
			control.Margin = new Thickness(prev.Left, prev.Top, prev.Right, value);
		});
		HorizontalMarginProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Control control || double.IsNaN(value))
				return;
			var prev = control.Margin;
			control.Margin = new Thickness(value, prev.Top, value, prev.Bottom);
		});
		VerticalMarginProperty.Changed.Subscribe(args =>
		{
			var element = args.Sender;
			var value = args.NewValue.Value;
			if (element is not Control control || double.IsNaN(value))
				return;
			var prev = control.Margin;
			control.Margin = new Thickness(prev.Left, value, prev.Right, value);
		});
	}

	public static void SetLeftMargin(AvaloniaObject element, double value)
	{
		element.SetValue(LeftMarginProperty, value);
		// if (element is not Control control || double.IsNaN(value))
		// 	return;
		// var prev = control.Margin;
		// control.Margin = new Thickness(value, prev.Top, prev.Right, prev.Bottom);
	}

	public static void SetTopMargin(AvaloniaObject element, double value)
	{
		element.SetValue(TopMarginProperty, value);
		// if (element is not Control control || double.IsNaN(value))
		// 	return;
		// var prev = control.Margin;
		// control.Margin = new Thickness(prev.Left, value, prev.Right, prev.Bottom);
	}

	public static void SetRightMargin(AvaloniaObject element, double value)
	{
		element.SetValue(RightMarginProperty, value);
		// if (element is not Control control || double.IsNaN(value))
		// 	return;
		// var prev = control.Margin;
		// control.Margin = new Thickness(prev.Left, prev.Top, value, prev.Bottom);
	}

	public static void SetBottomMargin(AvaloniaObject element, double value)
	{
		element.SetValue(BottomMarginProperty, value);
		// if (element is not Control control || double.IsNaN(value))
		// 	return;
		// var prev = control.Margin;
		// control.Margin = new Thickness(prev.Left, prev.Top, prev.Right, value);
	}

	public static void SetVerticalMargin(AvaloniaObject element, double value)
	{
		element.SetValue(VerticalMarginProperty, value);
		// if (element is not Control control || double.IsNaN(value))
		// 	return;
		// var prev = control.Margin;
		// control.Margin = new Thickness(prev.Left, value, prev.Right, value);
	}

	public static void SetHorizontalMargin(AvaloniaObject element, double value)
	{
		element.SetValue(HorizontalMarginProperty, value);
		// if (element is not Control control || double.IsNaN(value))
		// 	return;
		// var prev = control.Margin;
		// control.Margin = new Thickness(value, prev.Top, value, prev.Bottom);
	}

	public static void SetLeftPadding(AvaloniaObject element, double value)
	{
		element.SetValue(LeftPaddingProperty, value);
		// if (element is not Decorator decorator || double.IsNaN(value))
		// 	return;
		// var prev = decorator.Padding;
		// decorator.Padding = new Thickness(value, prev.Top, prev.Right, prev.Bottom);
	}

	public static void SetTopPadding(AvaloniaObject element, double value)
	{
		element.SetValue(TopPaddingProperty, value);
		// if (element is not Decorator decorator || double.IsNaN(value))
		// 	return;
		// var prev = decorator.Padding;
		// decorator.Padding = new Thickness(prev.Left, value, prev.Right, prev.Bottom);
	}

	public static void SetRightPadding(AvaloniaObject element, double value)
	{
		element.SetValue(RightPaddingProperty, value);
		// if (element is not Decorator decorator || double.IsNaN(value))
		// 	return;
		// var prev = decorator.Padding;
		// decorator.Padding = new Thickness(prev.Left, prev.Top, value, prev.Bottom);
	}

	public static void SetBottomPadding(AvaloniaObject element, double value)
	{
		element.SetValue(BottomPaddingProperty, value);
		// if (element is not Decorator decorator || double.IsNaN(value))
		// 	return;
		// var prev = decorator.Padding;
		// decorator.Padding = new Thickness(prev.Left, prev.Top, prev.Right, value);
	}

	public static void SetVerticalPadding(AvaloniaObject element, double value)
	{
		element.SetValue(VerticalPaddingProperty, value);
		// if (element is not Decorator decorator || double.IsNaN(value))
		// 	return;
		// var prev = decorator.Padding;
		// decorator.Padding = new Thickness(prev.Left, value, prev.Right, value);
	}

	public static void SetHorizontalPadding(AvaloniaObject element, double value)
	{
		element.SetValue(HorizontalPaddingProperty, value);
		// if (element is not Decorator decorator || double.IsNaN(value))
		// 	return;
		// var prev = decorator.Padding;
		// decorator.Padding = new Thickness(value, prev.Top, value, prev.Bottom);
	}
}