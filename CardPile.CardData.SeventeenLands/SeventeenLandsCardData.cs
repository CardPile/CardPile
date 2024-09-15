using CardPile.CardData.Formatting;

namespace CardPile.CardData.SeventeenLands;

internal class SeventeenLandsCardData : ICardData
{
    internal SeventeenLandsCardData(string name, int mtga_id, List<Color> colors) : this(new SeventeenLandsRawCardData(name, mtga_id, colors, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null), null, null, null, null, null, null, null, null, null, null)
    { }

    internal SeventeenLandsCardData(string name, int mtga_id, List<Color> colors, string? url) : this(new SeventeenLandsRawCardData(name, mtga_id, colors, url, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null), null, null, null, null, null, null, null, null, null, null)
    { }

    internal SeventeenLandsCardData
    (
        SeventeenLandsRawCardData rawCardData,
        float? wuEverDrawnWinRate,
        float? wbEverDrawnWinRate,
        float? wrEverDrawnWinRate,
        float? wgEverDrawnWinRate,
        float? ubEverDrawnWinRate,
        float? urEverDrawnWinRate,
        float? ugEverDrawnWinRate,
        float? brEverDrawnWinRate,
        float? bgEverDrawnWinRate,
        float? rgEverDrawnWinRate
    )
    {
        this.rawCardData = rawCardData;

        var seenMetric = rawCardData.SeenMetric;
        var averageLastSeenAtMetric = rawCardData.AverageLastSeenAtMetric;
        var pickedMetric = rawCardData.PickedMetric;
        var averagePickedAtMetric = rawCardData.AveragePickedAtMetric;
        var numberOfGamesPlayedMetric = rawCardData.NumberOfGamesPlayedMetric;
        var playRateMetric = rawCardData.PlayRateMetric;
        var winRateWhenMaindeckedMetric = rawCardData.WinRateWhenMaindeckedMetric;
        var numberOfGamesInOpeningHandMetric = rawCardData.NumberOfGamesInOpeningHandMetric;
        var winRateInOpeningHandMetric = rawCardData.WinRateInOpeningHandMetric;
        var numberOfGamesDrawnTurn1OrLaterMetric = rawCardData.NumberOfGamesDrawnTurn1OrLaterMetric;
        var winRateWhenDrawnTurn1OrLaterMetric = rawCardData.WinRateWhenDrawnTurn1OrLaterMetric;
        var numberOfGamesInHandMetric = rawCardData.NumberOfGamesInHandMetric;
        var winRateInHandMetric = rawCardData.WinRateInHandMetric;
        var colorsWinRateInHandMetric = ColorsWinRateInHandMetricDesc.NewMetricWithSort(rawCardData.WinRateInHandMetric,
                                                                                        WUWinRateInHandMetricDesc.NewMetric(wuEverDrawnWinRate),
                                                                                        WBWinRateInHandMetricDesc.NewMetric(wbEverDrawnWinRate),
                                                                                        WRWinRateInHandMetricDesc.NewMetric(wrEverDrawnWinRate),
                                                                                        WGWinRateInHandMetricDesc.NewMetric(wgEverDrawnWinRate),
                                                                                        UBWinRateInHandMetricDesc.NewMetric(ubEverDrawnWinRate),
                                                                                        URWinRateInHandMetricDesc.NewMetric(urEverDrawnWinRate),
                                                                                        UGWinRateInHandMetricDesc.NewMetric(ugEverDrawnWinRate),
                                                                                        BRWinRateInHandMetricDesc.NewMetric(brEverDrawnWinRate),
                                                                                        BGWinRateInHandMetricDesc.NewMetric(bgEverDrawnWinRate),
                                                                                        RGWinRateInHandMetricDesc.NewMetric(rgEverDrawnWinRate));
        var numberOfGamesNotSeenMetric = rawCardData.NumberOfGamesNotSeenMetric;
        var winRateNotSeenMetric = rawCardData.WinRateNotSeenMetric;
        var winRateImprovementWhenDrawnMetric = rawCardData.WinRateImprovementWhenDrawnMetric;

        Metrics =
        [
            seenMetric,
            averageLastSeenAtMetric,
            pickedMetric,
            averagePickedAtMetric,
            numberOfGamesPlayedMetric,
            playRateMetric,
            winRateWhenMaindeckedMetric,
            numberOfGamesInOpeningHandMetric,
            winRateInOpeningHandMetric,
            numberOfGamesDrawnTurn1OrLaterMetric,
            winRateWhenDrawnTurn1OrLaterMetric,
            numberOfGamesInHandMetric,
            winRateInHandMetric,
            colorsWinRateInHandMetric,
            numberOfGamesNotSeenMetric,
            winRateNotSeenMetric,
            winRateImprovementWhenDrawnMetric,
        ];
    }

    public string Name { get => rawCardData.Name; }

    public int ArenaCardId { get => rawCardData.ArenaCardId; }

    public List<Color> Colors { get => rawCardData.Colors; }

    public string? Url { get => rawCardData.Url; set => rawCardData.Url = value; }

    public List<ICardMetric> Metrics { get; init; }

    internal static List<ICardMetricDescription> MetricDescriptions
    {
        get
        {
            return
            [
                SeventeenLandsRawCardData.SeenMetricDesc,
                SeventeenLandsRawCardData.AverageLastSeenAtMetricDesc,
                SeventeenLandsRawCardData.PickedMetricDesc,
                SeventeenLandsRawCardData.AveragePickedAtMetricDesc,
                SeventeenLandsRawCardData.NumberOfGamesPlayedMetricDesc,
                SeventeenLandsRawCardData.PlayRateMetricDesc,
                SeventeenLandsRawCardData.WinRateWhenMaindeckedMetricDesc,
                SeventeenLandsRawCardData.NumberOfGamesInOpeningHandMetricDesc,
                SeventeenLandsRawCardData.WinRateInOpeningHandMetricDesc,
                SeventeenLandsRawCardData.NumberOfGamesDrawnTurn1OrLaterMetricDesc,
                SeventeenLandsRawCardData.WinRateWhenDrawnTurn1OrLaterMetricDesc,
                SeventeenLandsRawCardData.NumberOfGamesInHandMetricDesc,
                SeventeenLandsRawCardData.WinRateInHandMetricDesc,
                ColorsWinRateInHandMetricDesc,
                SeventeenLandsRawCardData.NumberOfGamesNotSeenMetricDesc,
                SeventeenLandsRawCardData.WinRateNotSeenMetricDesc,
                SeventeenLandsRawCardData.WinRateImprovementWhenDrawnMetricDesc,
            ];
        }
    }

    private static readonly CardMetricDescription<float> WUWinRateInHandMetricDesc = new CardMetricDescription<float>("WU", true, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float> WBWinRateInHandMetricDesc = new CardMetricDescription<float>("WB", true, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float> WRWinRateInHandMetricDesc = new CardMetricDescription<float>("WR", true, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float> WGWinRateInHandMetricDesc = new CardMetricDescription<float>("WG", true, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float> UBWinRateInHandMetricDesc = new CardMetricDescription<float>("UB", true, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float> URWinRateInHandMetricDesc = new CardMetricDescription<float>("UR", true, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float> UGWinRateInHandMetricDesc = new CardMetricDescription<float>("UG", true, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float> BRWinRateInHandMetricDesc = new CardMetricDescription<float>("BR", true, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float> BGWinRateInHandMetricDesc = new CardMetricDescription<float>("BG", true, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float> RGWinRateInHandMetricDesc = new CardMetricDescription<float>("RG", true, false, new CardMetricPercentFormatter());
    private static readonly CompositeCardMetricDescription ColorsWinRateInHandMetricDesc = new CompositeCardMetricDescription("GIH WR (2C)", false, false,
                                                                                                                               WUWinRateInHandMetricDesc,
                                                                                                                               WBWinRateInHandMetricDesc,
                                                                                                                               WRWinRateInHandMetricDesc,
                                                                                                                               WGWinRateInHandMetricDesc,
                                                                                                                               UBWinRateInHandMetricDesc,
                                                                                                                               URWinRateInHandMetricDesc,
                                                                                                                               UGWinRateInHandMetricDesc,
                                                                                                                               BRWinRateInHandMetricDesc,
                                                                                                                               BGWinRateInHandMetricDesc,
                                                                                                                               RGWinRateInHandMetricDesc);

    private SeventeenLandsRawCardData rawCardData;
}
