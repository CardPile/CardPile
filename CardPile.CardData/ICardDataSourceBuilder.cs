namespace CardPile.CardData;

public interface ICardDataSourceBuilder
{
    public string Name { get; }

    public List<ICardDataSourceParameter> Parameters { get; }

    public List<ICardMetricDescription> MetricDescriptions { get; }

    // TODO: Hook this up to draft start
    // ICardDataSource BuildForSetAndEvent(string set, string event);

    ICardDataSource Build();

    Task<ICardDataSource> BuildAsync(CancellationToken cancelation);
}
