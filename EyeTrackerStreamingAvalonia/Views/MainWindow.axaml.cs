using Avalonia.Controls;

namespace EyeTrackerStreamingAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        Avalonia.DevToolsExtensions.AttachDevTools(this);
#endif
    }
}