namespace EyeTrackerStreaming.Shared.NullObjects;

internal sealed class NullDisposable : IDisposable
{
    public static readonly IDisposable Instance = new NullDisposable();
    private NullDisposable() {}
    public void Dispose() { }
}