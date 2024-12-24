using CardPile.CardData.Metrics;

namespace CardPile.CardData.Spreadsheet;

public class CardData : ICardData
{
    internal CardData(string name, int arenaCardId, int? manaValue, List<Color> colors, string? url, string? grade = null)
    {
        Name = name;
        ArenaCardId = arenaCardId;
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

    public int? ManaValue { get; init; }
    
    public List<Color> Colors { get; init; }

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

