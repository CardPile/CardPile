using CardPile.CardData.Formatting;

namespace CardPile.CardData;

public class CardMetricDescription<T> : ICardMetricDescription where T : struct 
{
    public CardMetricDescription(string name, bool isDefaultVisible, bool isDefault, ICardMetricFormatter<T>? formatter = null)
    {
        Name = name;
        IsDefaultVisible = isDefaultVisible;
        IsDefaultMetric = isDefault;
        Formatter = formatter;
    }

    public string Name { get; init; }

    public bool IsDefaultVisible { get; init; }

    public bool IsDefaultMetric { get; init; }

    public IComparer<ICardMetric> Comparer { get => new CardMetricComparer(); }

    public ICardMetric NewMetric<E>(E? value)
    {
        if(value == null)
        {
            return new CardMetric<T>(this, null);
        }

        if (value is not T baseValue)
        {
            throw new ArgumentException("The metric value type must match the metric description type");
        }
        return new CardMetric<T>(this, baseValue);
    }

    internal ICardMetricFormatter<T>? Formatter { get; init; }

    private class CardMetricComparer : IComparer<ICardMetric>
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

            if (x is not CardMetric<T> xMetric)
            {
                throw new ArgumentException("First comparer parameter is not a CardMetric<T>");
            }

            if (y is not CardMetric<T> yMetric)
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


