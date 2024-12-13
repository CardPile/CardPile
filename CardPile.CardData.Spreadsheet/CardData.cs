using CardPile.CardData.Metrics;
using CardPile.Draft;

namespace CardPile.CardData.Spreadsheet;

public class CardData : ICardData
{
    internal CardData(string name, int arenaCardId, List<Color> colors, string? url) : this(name, arenaCardId, colors, url, null)
    { }

    internal CardData(string name, int arenaCardId, List<Color> colors, string? url, string? grade)
    {
        Name = name;
        ArenaCardId = arenaCardId;
        Colors = colors;
        Url = url;
        Metrics =
        [
            GradeMetricDesc.NewMetric(grade),
        ];
    }

    public string Name { get; init; }

    public int ArenaCardId { get; init; }

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

