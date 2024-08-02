namespace CardPile.CardData;

public interface ICardMetricDescription
{
    public string Name { get; }

    public bool IsDefaultVisible { get; }

    public bool IsDefaultMetric { get; }

    public IComparer<ICardMetric> Comparer { get; }

    public ICardMetric NewMetric<T>(T? value);
}
