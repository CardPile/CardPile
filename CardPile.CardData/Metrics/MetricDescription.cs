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

    public IComparer<ICardMetric> Comparer { get => new CompositeCardMetricComparer(); }

    public Metric<T> NewMetric<E>(E? value)
    {
        return NewMetric<E>(value, (T) => ImportanceLevel.Regular);
    }

    public Metric<T> NewMetric<E>(E? value, Func<T, ImportanceLevel> importanceCalculator)
    {
        if (value == null)
        {
            return new Metric<T>(this, null, ImportanceLevel.Regular);
        }

        if (value is not T baseValue)
        {
            throw new ArgumentException("The metric value type must match the metric description type");
        }

        return new Metric<T>(this, baseValue, importanceCalculator(baseValue));
    }

    public Metric<T> NewMetric()
    {
        return new Metric<T>(this, null, ImportanceLevel.Regular);
    }

    internal IFormatter<T>? Formatter { get; init; }

    private class CompositeCardMetricComparer : IComparer<ICardMetric>
    {
        public int Compare(ICardMetric? x, ICardMetric? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x is not Metric<T> xMetric)
            {
                throw new ArgumentException("First comparer parameter is not a CardMetric<T>");
            }

            if (y is not Metric<T> yMetric)
            {
                throw new ArgumentException("Second comparer parameter is not a CardMetric<T>");
            }

            if (!xMetric.Value.HasValue && !yMetric.Value.HasValue)
            {
                return 0;
            }

            if (!xMetric.Value.HasValue)
            {
                return -1;
            }

            if (!yMetric.Value.HasValue)
            {
                return 1;
            }

            return Comparer<T>.Default.Compare(xMetric.Value.Value, yMetric.Value.Value);
        }
    };
}


