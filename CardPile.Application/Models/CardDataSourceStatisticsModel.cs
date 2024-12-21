using Avalonia.Controls.Templates;
using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CardPile.Application.Models;

internal class CardDataSourceStatisticsModel : ReactiveObject, ICardDataSourceStatisticsService
{
    public CardDataSourceStatisticsModel(ICardDataSource cardDataSource)
    {
        SetCardDataSource(cardDataSource);
    }

    public ObservableCollection<ICardDataSourceStatisticService> Statistics
    {
        get => statistics;
        set => this.RaiseAndSetIfChanged(ref statistics, value);
    }

    [MemberNotNull(nameof(cardDataSource))]
    internal void SetCardDataSource(ICardDataSource newCardDataSource)
    {
        cardDataSource = newCardDataSource;
        UpdateStatistics(cardDataSource);
    }

    private void UpdateStatistics(ICardDataSource cardDataSource)
    {
        statistics.Clear();

        foreach (var statistic in cardDataSource.Statistics)
        {
            statistics.Add(new CardDataSourceStatisticModel(statistic));
        }
    }

    private ObservableCollection<ICardDataSourceStatisticService> statistics = [];

    private ICardDataSource cardDataSource;
}
