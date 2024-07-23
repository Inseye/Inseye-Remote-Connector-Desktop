using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace EyeTrackerStreamingAvalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{

    public string Greeting => "Welcome to Avalonia!";
    public IBrush Mice { get; } = new ImmutableSolidColorBrush(Colors.Black, 0.8d);
    public MainWindowViewModel()
    {
        
    }
    
}