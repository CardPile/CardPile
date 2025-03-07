namespace CardPile.CardData.Metrics;

public class MetricComparer<T> : IComparer<ICardMetric> where T : struct
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