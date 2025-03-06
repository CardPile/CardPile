using CardPile.CardData.Formatting;
using CardPile.CardData.Importance;

namespace CardPile.CardData.Metrics;

public class MetricDescription<T> : ICardMetricDescription where T : struct
{
    public MetricDescription(string name, bool isDefaultVisible, bool isDefault, IFormatter<T>? formatter = null)
    {
        Name = name;
        IsDefaultVisible = isDefaultVisible;
        IsDefaultMetric = isDefault;
        Formatter = formatter;
    }

    public string Name { get; init; }

    public bool IsDefaultVisible { get; init; }

    public bool IsDefaultMetric { get; init; }

    public IComparer<ICardMetric> Comparer { get => new MetricComparer<T>(); }

    public Metric<T> NewMetric<E>(E? value, List<ICardRank> ranks)
    {
        return NewMetric<E>(value, (T) => ImportanceLevel.Regular, ranks);
    }

    public Metric<T> NewMetric<E>(E? value, Func<T, ImportanceLevel> importanceCalculator, List<ICardRank> ranks)
    {
        if (value == null)
        {
            return new Metric<T>(this, null, ImportanceLevel.Regular, ranks);
        }

        if (value is not T baseValue)
        {
            throw new ArgumentException("The metric value type must match the metric description type");
        }

        return new Metric<T>(this, baseValue, importanceCalculator(baseValue), ranks);
    }

    public Metric<T> NewMetric()
    {
        return new Metric<T>(this, null, ImportanceLevel.Regular, []);
    }

    internal IFormatter<T>? Formatter { get; init; }
}


