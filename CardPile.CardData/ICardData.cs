namespace CardPile.CardData;

public interface ICardData
{
    public string Name { get; }

    public int ArenaCardId { get; }

    public List<Color> Colors { get; }

    public string? Url { get; }

    public List<ICardMetric> Metrics { get; }
};