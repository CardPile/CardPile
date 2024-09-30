using CardPile.CardData.Importance;

namespace CardPile.CardData.Metrics;

public class LetterGradeMetric : ICardMetric
{
    public LetterGradeMetric(LetterGradeMetricDescription description, string value, ImportanceLevel importance)
    {
        Description = description;
        Importance = importance;
        Value = value;
    }

    public ICardMetricDescription Description { get; init; }

    public bool HasValue { get => Value != null; }

    public string TextValue { get => Value ?? string.Empty; }

    public ImportanceLevel Importance { get; init; }

    public string Value { get; init; }
}
