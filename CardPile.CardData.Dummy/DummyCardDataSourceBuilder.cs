namespace CardPile.CardData.Dummy;

public class DummyCardDataSourceBuilder : ICardDataSourceBuilder
{
    public string Name => "Dummy";

    public List<ICardDataSourceSetting> Settings => [FirstSetting];

    public List<ICardDataSourceParameter> Parameters => [FirstParameter, SecondParameter];

    public List<ICardMetricDescription> MetricDescriptions { get; init; } = [MetricADesc, MetricBDesc, MetricCDesc, MetricDDesc, MetricEDesc];

    public ICardDataSource Build()
    {
        return new DummyCardDataSource();
    }

    public Task<ICardDataSource> BuildAsync(CancellationToken cancelation)
    {
        return Task.FromResult(new DummyCardDataSource() as ICardDataSource);
    }

    internal static CardMetricDescription<float> MetricADesc { get; } = new CardMetricDescription<float>("Metric A", false, false);
    internal static CardMetricDescription<int> MetricBDesc { get; } = new CardMetricDescription<int>("Metric B", true, false);
    internal static CardMetricDescription<float> MetricCDesc { get; } = new CardMetricDescription<float>("Metric C", true, true);
    internal static CardLetterGradeMetricDescription MetricDDesc { get; } = new CardLetterGradeMetricDescription("Metric D", true, false);
    internal static CompositeCardMetricDescription MetricEDesc { get; } = new CompositeCardMetricDescription("Metic ACD", true, false, MetricADesc, MetricCDesc, MetricDDesc);

    private CardDataSourceSettingPath FirstSetting = new CardDataSourceSettingPath("Test path", "Foo");

    private CardDataSourceParameterOptions FirstParameter = new CardDataSourceParameterOptions("First", ["Option A", "Option B"]);
    private CardDataSourceParameterOptions SecondParameter = new CardDataSourceParameterOptions("Second", ["Option 1", "Option 2", "Option 3"]);
}
