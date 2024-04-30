// Module name: TerminalGUI
// File name: ErrorWindow.cs
// Last edit: 2024-04-30 12:21 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Terminal.Gui;

namespace TerminalGUI.Views;

public sealed class ErrorWindow : Window
{
    private ErrorWindow(View parent)
    {
        Width = Dim.Percent(75);
        Height = Dim.Percent(75);
        X = Pos.Center();
        Y = Pos.Center();
        ColorScheme = new ColorScheme(Terminal.Gui.Attribute.Default);
        Title = "Error";
        Parent = parent;
        ErrorTextField = new Label
        {
            X = Pos.Center(),
            Y = Pos.Center()
        };
        Add(ErrorTextField);
        var button = new Button
        {
            X = Pos.Center(),
            Y = Pos.AnchorEnd(2),
            Text = "Close"
        };
        button.Clicked += Close;
        Add(button);
        Parent.Add(this);
        SuperView.BringSubviewToFront(this);
        SetFocus();
    }

    private View Parent { get; }
    private Label ErrorTextField { get; }

    public static IDisposable ShowError(View container, string message)
    {
        var errorBox = new ErrorWindow(container);
        errorBox.ErrorTextField.Text = message;
        return errorBox;
    }

    private void Close(object? _, EventArgs __)
    {
        Parent.SetFocus();
        Parent.Remove(this);
        Dispose();
    }
}