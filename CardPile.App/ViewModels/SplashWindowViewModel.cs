using ReactiveUI;
using System.Threading;

namespace CardPile.App.ViewModels;

public class SplashWindowViewModel : ViewModelBase
{
    public string StartupMessage
    {
        get => startupMessage;
        set { this.RaiseAndSetIfChanged(ref startupMessage, value); }
    }

    public CancellationToken CancellationToken => source.Token;

    public void Cancel()
    {
        StartupMessage = "Cancelling...";
        source.Cancel();
    }

    private readonly CancellationTokenSource source = new CancellationTokenSource();
    private string startupMessage = "Starting...";
}
