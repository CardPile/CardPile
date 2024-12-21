using System.Collections.ObjectModel;

namespace CardPile.Application.Services;

internal interface ICardDataSourceStatisticsService
{
    public ObservableCollection<ICardDataSourceStatisticService> Statistics { get; }
}
