using System.Text;
using System.Threading.Tasks;
using CardPile.CardData.Importance;

namespace CardPile.CardData.Metrics;

public class CompositeMetric : ICardMetric
{
    public CompositeMetric(CompositeMetricDescription description, params ICardMetric?[] values)
    {
        if (values.Length == 0)
        {
            throw new ArgumentException("values must contain at least one element", nameof(values));
        }

        if (values.Length != description.Descriptions.Count)
        {
            throw new ArgumentException("values must have the same number of elements as description count", nameof(values));
        }

        Description = description;
        Importance = InferImportance(values);
        Values = new List<ICardMetric?>(values);
        SortValue = Values.FirstOrDefault();
    }

    public ICardMetricDescription Description { get; init; }

    public bool HasValue { get => Values.Any(value => value != null && value.HasValue); }

    public string TextValue
    {
        get
        {
            StringBuilder builder = new();
            foreach (ICardMetric? metric in Values)
            {
                if (metric == null)
                {
                    continue;
                }

                if (!metric.HasValue)
                {
                    continue;
                }

                builder.AppendFormat("{0}\t{1}\n", metric.Description.Name, metric.TextValue);
            }
            return builder.ToString();
        }
    }

    public ImportanceLevel Importance { get; init; }

    internal List<ICardMetric?> Values { get; init; }

    internal ICardMetric? SortValue { get; set; }

    private static ImportanceLevel InferImportance(params ICardMetric?[] values)
    {
        ImportanceLevel? result = null;
        foreach (ICardMetric? metric in values)
        {
            if (metric == null)
            {
                continue;
            }

            result = ((result ?? ImportanceLevel.Low) < metric.Importance ? metric.Importance : result);
        }

        return result ?? ImportanceLevel.Regular;
    }
}
