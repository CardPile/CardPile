using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CardPile.App.Services;

internal interface ICardDataSourceBuilderService : IReactiveObject
{
    public string Name { get; }

    public List<ICardDataSourceSettingService> Settings { get; }

    public List<ICardDataSourceParameterService> Parameters { get; }

    public List<ICardMetricDescriptionService> MetricDescriptions { get; }

    public ICardDataSource BuildDataSource();

    public Task<ICardDataSource> BuildDataSourceAsync(CancellationToken cancellation);
}
