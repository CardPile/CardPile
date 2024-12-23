using CardPile.Application.Services;

namespace CardPile.Application.ViewModels;

public class CardViewModelBase : ViewModelBase
{
    internal CardViewModelBase(ICardDataService cardDataService)
    {
        CardDataService = cardDataService;
    }

    internal ICardDataService CardDataService { get; init; }
}
