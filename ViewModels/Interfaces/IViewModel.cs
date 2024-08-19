using ReactiveUI;

namespace EyeTrackerStreamingAvalonia.ViewModels;

public interface IViewModel : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
{
}