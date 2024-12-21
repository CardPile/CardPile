using CardPile.Application.Services;

namespace CardPile.Application.ViewModels;

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
