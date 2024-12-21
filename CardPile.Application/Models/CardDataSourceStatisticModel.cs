using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;

namespace CardPile.Application.Models;

internal class CardDataSourceStatisticModel : ReactiveObject, ICardDataSourceStatisticService
{
    internal CardDataSourceStatisticModel(ICardDataSourceStatistic statistic)
    {
        this.statistic = statistic;
    }

    public string Name { get => statistic.Name; }

    public string TextValue { get => statistic.TextValue; }

    private ICardDataSourceStatistic statistic;
}

