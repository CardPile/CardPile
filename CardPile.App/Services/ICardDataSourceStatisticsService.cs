using System.Collections.ObjectModel;

namespace CardPile.App.Services;

internal interface ICardDataSourceStatisticsService
{
    public ObservableCollection<ICardDataSourceStatisticService> Statistics { get; }
}
