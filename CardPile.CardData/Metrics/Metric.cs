using CardPile.CardData.Importance;

namespace CardPile.CardData.Metrics;

public class Metric<T> : ICardMetric where T : struct
{
    public Metric(MetricDescription<T> description, T? value, ImportanceLevel importance)
    {
        this.description = description;

        Importance = importance;

        Value = value;
    }

    public ICardMetricDescription Description { get => description; }

    public bool HasValue { get => Value != null; }

    public string TextValue
    {
        get
        {
            if (description.Formatter != null)
            {
                return description.Formatter.Format(Value);
            }
            else
            {
                return Value?.ToString() ?? string.Empty;
            }
        }
    }

    public ImportanceLevel Importance { get; init; }

    public T? Value { get; init; }

    private MetricDescription<T> description;
}
