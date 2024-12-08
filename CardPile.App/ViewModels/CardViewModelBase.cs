using CardPile.App.Services;

namespace CardPile.App.ViewModels;

public class CardViewModelBase : ViewModelBase
{
    internal CardViewModelBase(ICardDataService cardDataService)
    {
        CardDataService = cardDataService;
        ShowLabel = true;
    }

    internal ICardDataService CardDataService { get; init; }

    internal bool ShowLabel { get; set; }
}
