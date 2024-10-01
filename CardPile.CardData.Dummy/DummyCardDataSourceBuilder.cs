using CardPile.CardData.Metrics;
using CardPile.CardData.Parameters;
using CardPile.CardData.Settings;

namespace CardPile.CardData.Dummy;

public class DummyCardDataSourceBuilder : ICardDataSourceBuilder
{
    public static void Init()
    {
        // NOOP
    }

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

    internal static MetricDescription<float> MetricADesc { get; } = new MetricDescription<float>("Metric A", false, false);
    internal static MetricDescription<int> MetricBDesc { get; } = new MetricDescription<int>("Metric B", true, false);
    internal static MetricDescription<float> MetricCDesc { get; } = new MetricDescription<float>("Metric C", true, true);
    internal static LetterGradeMetricDescription MetricDDesc { get; } = new LetterGradeMetricDescription("Metric D", true, false);
    internal static CompositeMetricDescription MetricEDesc { get; } = new CompositeMetricDescription("Metic ACD", true, false, MetricADesc, MetricCDesc, MetricDDesc);

    private SettingPath FirstSetting = new SettingPath("Test path", "Foo");

    private ParameterOptions FirstParameter = new ParameterOptions("First", ["Option A", "Option B"]);
    private ParameterOptions SecondParameter = new ParameterOptions("Second", ["Option 1", "Option 2", "Option 3"]);
}
