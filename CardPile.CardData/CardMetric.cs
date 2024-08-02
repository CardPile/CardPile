namespace CardPile.CardData;

public class CardMetric<T> : ICardMetric where T : struct
{
    public CardMetric(CardMetricDescription<T> description, T? value)
    {
        this.description = description;

        Value = value;
    }

    public ICardMetricDescription Description { get => description; }

    public bool HasValue { get => Value != null; }

    public string TextValue
    {
        get
        {
            if(description.Formatter != null)
            {
                return description.Formatter.Format(Value);
            }
            else
            {
                return Value?.ToString() ?? string.Empty;
            }
        }
    }

    internal T? Value { get; init; }

    private CardMetricDescription<T> description;
}
