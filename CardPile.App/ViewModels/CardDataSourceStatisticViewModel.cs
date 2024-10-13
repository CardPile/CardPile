using CardPile.App.Services;

namespace CardPile.App.ViewModels;

public class CardDataSourceStatisticViewModel : ViewModelBase
{
    internal CardDataSourceStatisticViewModel(ICardDataSourceStatisticService cardDataSourceStatisticService)
    {
        this.cardDataSourceStatisticService = cardDataSourceStatisticService;
    }

    internal ICardDataSourceStatisticService CardDataSourceStatisticService
    {
        get => cardDataSourceStatisticService;
    }

    public string Name
    {
        get => cardDataSourceStatisticService.Name;
    }

    public string TextValue
    {
        get => cardDataSourceStatisticService.TextValue;
    }

    private ICardDataSourceStatisticService cardDataSourceStatisticService;
}
