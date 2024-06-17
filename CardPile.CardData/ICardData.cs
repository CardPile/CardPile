using CardPile.CardData;

public interface ICardData
{
    public string Name { get; }

    public int ArenaCardId { get; init; }

    public string? Url { get; }

    public List<ICardMetric> Metrics { get; }
};
