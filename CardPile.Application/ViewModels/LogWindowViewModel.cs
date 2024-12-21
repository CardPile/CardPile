using CardPile.Application.Services;

namespace CardPile.Application.ViewModels;

internal class LogWindowViewModel : ViewModelBase
{
    internal LogWindowViewModel(ILogService logService)
    {
        Log = logService;
    }

    internal ILogService Log { get; init; }
}
