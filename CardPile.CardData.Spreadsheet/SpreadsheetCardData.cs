namespace CardPile.CardData.Spreadsheet;

public class SpreadsheetCardData : ICardData
{
    internal SpreadsheetCardData(string name, int arenaCardId, string? url) : this(name, arenaCardId, url, null)
    { }

    internal SpreadsheetCardData(string name, int arenaCardId, string? url, string? grade)
    {
        Name = name;
        ArenaCardId = arenaCardId;
        Url = url;
        Metrics =
        [
            GradeMetricDesc.NewMetric(grade),
        ];
    }

    public string Name { get; init; }

    public int ArenaCardId { get; init; }

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

    private static readonly CardLetterGradeMetricDescription GradeMetricDesc = new CardLetterGradeMetricDescription("Grade", true, true);
}

