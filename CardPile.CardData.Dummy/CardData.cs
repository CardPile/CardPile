
namespace CardPile.CardData.Dummy;

public class CardData : ICardData
{
    internal CardData(string name, int arenaCardId, Type type, int? manaValue, List<Color> colors, string? url, ICardMetric? metricA, ICardMetric? metricB, ICardMetric? metricC, ICardMetric? metricD, ICardMetric? metricE)
    {
        Name = name;
        ArenaCardId = arenaCardId;
        Type = type;
        ManaValue = manaValue;
        Colors = colors;
        Url = url;
        Metrics =
        [
            metricA,
            metricB,
            metricC,
            metricD,
            metricE,
        ];
    }

    public string Name { get; init; }

    public int ArenaCardId { get; init; }

    public Type Type { get; init; }
    
    public int? ManaValue { get; init; }
    
    public List<Color> Colors { get; init; }

    public string? Url { get; init; }

    public List<ICardMetric> Metrics { get; init; }
}
