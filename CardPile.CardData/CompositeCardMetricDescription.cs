using CardPile.CardData.Formatting;

namespace CardPile.CardData;

public class CompositeCardMetricDescription : ICardMetricDescription
{
    public CompositeCardMetricDescription(string name, bool isDefaultVisible, bool isDefault, params ICardMetricDescription?[] descriptions)
    {
        if(descriptions.Length == 0)
        {
            throw new ArgumentException("descriptions must contain at least one element", nameof(descriptions));
        }

        Name = name;
        IsDefaultVisible = isDefaultVisible;
        IsDefaultMetric = isDefault;
        Descriptions = new List<ICardMetricDescription?>(descriptions);
    }

    public string Name { get; init; }

    public bool IsDefaultVisible { get; init; }

    public bool IsDefaultMetric { get; init; }

    public IComparer<ICardMetric> Comparer { get => new CardMetricComparer(); }

    public ICardMetric NewMetric(params ICardMetric?[] values)
    {
        return new CompositeCardMetric(this, values);
    }

    public ICardMetric NewMetricWithSort(ICardMetric? sortMetric, params ICardMetric?[] values)
    {
        var result = new CompositeCardMetric(this, values);
        result.SortValue = sortMetric;
        return result;
    }

    internal List<ICardMetricDescription?> Descriptions { get; init; }

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

            if (x is not CompositeCardMetric xMetric)
            {
                throw new ArgumentException("First comparer parameter is not a CompositeCardMetric");
            }

            if (y is not CompositeCardMetric yMetric)
            {
                throw new ArgumentException("Second comparer parameter is not a CompositeCardMetric");
            }

            ICardMetric? xFirstValue = xMetric.SortValue;
            ICardMetric? yFirstValue = yMetric.SortValue;

            if (xFirstValue == null && yFirstValue == null)
            {
                return 0;
            }

            if (xFirstValue == null)
            {
                return -1;
            }

            if (yFirstValue == null)
            {
                return 1;
            }

            return xFirstValue.Description.Comparer.Compare(xFirstValue, yFirstValue);
        }
    };
}


