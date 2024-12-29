using CardPile.CardData.Metrics;

namespace CardPile.CardData.Spreadsheet;

public class CardData : ICardData
{
    internal CardData(string name, int arenaCardId, Type type, int? manaValue, Color colors, string? url, string? grade = null)
    {
        Name = name;
        ArenaCardId = arenaCardId;
        Type = type;
        ManaValue = manaValue;
        Colors = colors;
        Url = url;
        Metrics =
        [
            GradeMetricDesc.NewMetric(grade),
        ];
    }

    public string Name { get; init; }

    public int ArenaCardId { get; init; }

    public Type Type { get; init; }
    
    public int? ManaValue { get; init; }
    
    public Color Colors { get; init; }

    public string? Url { get; init; }

    public List<ICardMetric> Metrics { get; init; }

    internal static List<ICardMetricDescription> MetricDescriptions
    {
        get
        {
            return
            [
                GradeMetricDesc,
            ];
        }
    }

    private static readonly LetterGradeMetricDescription GradeMetricDesc = new LetterGradeMetricDescription("Grade", true, true);
}

