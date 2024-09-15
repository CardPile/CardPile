namespace CardPile.CardData;

public class CardLetterGradeMetric : ICardMetric
{
    public CardLetterGradeMetric(CardLetterGradeMetricDescription description, string value)
    {
        Description = description;
        Value = value;
    }

    public ICardMetricDescription Description { get; init; }

    public bool HasValue { get => Value != null; }

    public string TextValue { get => Value ?? string.Empty; }

    internal string Value { get; init; }
}
