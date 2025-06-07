using ReactiveUI;
using System.Collections.ObjectModel;

namespace CardPile.Application.Services;

internal interface ICardDataSourceStatisticsService : IReactiveObject
{
    public ObservableCollection<ICardDataSourceStatisticService> Statistics { get; }
}
