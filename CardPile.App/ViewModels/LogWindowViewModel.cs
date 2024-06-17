using CardPile.App.Services;

namespace CardPile.App.ViewModels;

internal class LogWindowViewModel : ViewModelBase
{
    internal LogWindowViewModel(ILogService logService)
    {
        Log = logService;
    }

    internal ILogService Log { get; init; }
}
