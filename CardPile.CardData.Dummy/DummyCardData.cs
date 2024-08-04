
namespace CardPile.CardData.Dummy;

public class DummyCardData : ICardData
{
    internal DummyCardData(string name, int arenaCardId, List<Color> colors, string? url, ICardMetric? metricA, ICardMetric? metricB, ICardMetric? metricC, ICardMetric? metricD)
    {
        Name = name;
        ArenaCardId = arenaCardId;
        Colors = colors;
        Url = url;
        Metrics =
        [
            metricA,
            metricB,
            metricC,
            metricD,
        ];
    }

    public string Name { get; init; }

    public int ArenaCardId { get; init; }

    public List<Color> Colors { get; init; }

    public string? Url { get; init; }

    public List<ICardMetric> Metrics { get; init; }
}
