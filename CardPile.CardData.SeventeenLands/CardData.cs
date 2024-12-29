using CardPile.CardData.Formatting;
using CardPile.CardData.Metrics;

namespace CardPile.CardData.SeventeenLands;

internal class CardData : ICardData
{
    internal CardData(string name, int arenaCardId, Type type, int? manaValue, Color colors, string? url = null) : this(
        name,
        arenaCardId,
        type,
        manaValue, 
        colors,
        url,
        SeenMetricDesc.NewMetric(),
        AverageLastSeenAtMetricDesc.NewMetric(),
        PickedMetricDesc.NewMetric(),
        AveragePickedAtMetricDesc.NewMetric(),
        NumberOfGamesPlayedMetricDesc.NewMetric(),
        PlayRateMetricDesc.NewMetric(),
        WinRateWhenMaindeckedMetricDesc.NewMetric(),
        NumberOfGamesInOpeningHandMetricDesc.NewMetric(),
        WinRateInOpeningHandMetricDesc.NewMetric(),
        NumberOfGamesDrawnTurn1OrLaterMetricDesc.NewMetric(),
        WinRateWhenDrawnTurn1OrLaterMetricDesc.NewMetric(),
        NumberOfGamesInHandMetricDesc.NewMetric(),
        WinRateInHandMetricDesc.NewMetric(),
        ColorsWinRateInHandMetricDesc.NewMetric(
            WUWinRateInHandMetricDesc.NewMetric(),
            WBWinRateInHandMetricDesc.NewMetric(),
            WRWinRateInHandMetricDesc.NewMetric(),
            WGWinRateInHandMetricDesc.NewMetric(),
            UBWinRateInHandMetricDesc.NewMetric(),
            URWinRateInHandMetricDesc.NewMetric(),
            UGWinRateInHandMetricDesc.NewMetric(),
            BRWinRateInHandMetricDesc.NewMetric(),
            BGWinRateInHandMetricDesc.NewMetric(),
            RGWinRateInHandMetricDesc.NewMetric()
        ),
        ColorsWinRateImprovementMetricDesc.NewMetric(
            WUWinRateImprovementMetricDesc.NewMetric(),
            WBWinRateImprovementMetricDesc.NewMetric(),
            WRWinRateImprovementMetricDesc.NewMetric(),
            WGWinRateImprovementMetricDesc.NewMetric(),
            UBWinRateImprovementMetricDesc.NewMetric(),
            URWinRateImprovementMetricDesc.NewMetric(),
            UGWinRateImprovementMetricDesc.NewMetric(),
            BRWinRateImprovementMetricDesc.NewMetric(),
            BGWinRateImprovementMetricDesc.NewMetric(),
            RGWinRateImprovementMetricDesc.NewMetric()
        ),
        NumberOfGamesNotSeenMetricDesc.NewMetric(),
        WinRateNotSeenMetricDesc.NewMetric(),
        WinRateImprovementWhenDrawnMetricDesc.NewMetric())
    { }

    internal CardData
    (
        string name,
        int arenaCardId,
        Type type,
        int? manaValue,
        Color colors,
        string? url,
        ICardMetric seenMetric,
        ICardMetric averageLastSeenAtMetric,
        ICardMetric pickedMetric,
        ICardMetric averagePickedAtMetric,
        ICardMetric numberOfGamesPlayedMetric,
        ICardMetric playRateMetric,
        ICardMetric winRateWhenMaindeckedMetric,
        ICardMetric numberOfGamesInOpeningHandMetric,
        ICardMetric winRateInOpeningHandMetric,
        ICardMetric numberOfGamesDrawnTurn1OrLaterMetric,
        ICardMetric winRateWhenDrawnTurn1OrLaterMetric,
        ICardMetric numberOfGamesInHandMetric,
        ICardMetric winRateInHandMetric,
        ICardMetric colorsWinRateInHandMetric,
        ICardMetric colorsWinRateImprovementMetric,
        ICardMetric numberOfGamesNotSeenMetric,
        ICardMetric winRateNotSeenMetric,
        ICardMetric winRateImprovementWhenDrawnMetric
    )
    {
        Name = name;
        ArenaCardId = arenaCardId;
        Type = type;
        ManaValue = manaValue;
        Colors = colors;
        Url = url;
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
            colorsWinRateImprovementMetric,
            numberOfGamesNotSeenMetric,
            winRateNotSeenMetric,
            winRateImprovementWhenDrawnMetric,
        ];
    }

    public string Name { get; init; }

    public int ArenaCardId { get; init; }

    public Type Type { get; init; }
    
    public int? ManaValue { get; init; }
    
    public Color Colors { get; init; }

    public string? Url { get; set; }

    public List<ICardMetric> Metrics { get; init; }

    internal static List<ICardMetricDescription> MetricDescriptions
    {
        get
        {
            return
            [
                SeenMetricDesc,
                AverageLastSeenAtMetricDesc,
                PickedMetricDesc,
                AveragePickedAtMetricDesc,
                NumberOfGamesPlayedMetricDesc,
                PlayRateMetricDesc,
                WinRateWhenMaindeckedMetricDesc,
                NumberOfGamesInOpeningHandMetricDesc,
                WinRateInOpeningHandMetricDesc,
                NumberOfGamesDrawnTurn1OrLaterMetricDesc,
                WinRateWhenDrawnTurn1OrLaterMetricDesc,
                NumberOfGamesInHandMetricDesc,
                WinRateInHandMetricDesc,
                ColorsWinRateInHandMetricDesc,
                ColorsWinRateImprovementMetricDesc,
                NumberOfGamesNotSeenMetricDesc,
                WinRateNotSeenMetricDesc,
                WinRateImprovementWhenDrawnMetricDesc,
            ];
        }
    }

    internal static readonly MetricDescription<int> SeenMetricDesc = new MetricDescription<int>("# Seen", false, false);
    internal static readonly MetricDescription<float> AverageLastSeenAtMetricDesc = new MetricDescription<float>("ALSA", true, false, new DecimalFormatter());
    internal static readonly MetricDescription<int> PickedMetricDesc = new MetricDescription<int>("# Picked", false, false);
    internal static readonly MetricDescription<float> AveragePickedAtMetricDesc = new MetricDescription<float>("ATA", true, false, new DecimalFormatter());
    internal static readonly MetricDescription<int> NumberOfGamesPlayedMetricDesc = new MetricDescription<int>("# GP", false, false);
    internal static readonly MetricDescription<float> PlayRateMetricDesc = new MetricDescription<float>("% GP", false, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WinRateWhenMaindeckedMetricDesc = new MetricDescription<float>("GP WR", true, false, new PercentFormatter());
    internal static readonly MetricDescription<int> NumberOfGamesInOpeningHandMetricDesc = new MetricDescription<int>("# OH", false, false);
    internal static readonly MetricDescription<float> WinRateInOpeningHandMetricDesc = new MetricDescription<float>("OH WR", false, false, new PercentFormatter());
    internal static readonly MetricDescription<int> NumberOfGamesDrawnTurn1OrLaterMetricDesc = new MetricDescription<int>("# GD", false, false);
    internal static readonly MetricDescription<float> WinRateWhenDrawnTurn1OrLaterMetricDesc = new MetricDescription<float>("GD WR", false, false, new PercentFormatter());
    internal static readonly MetricDescription<int> NumberOfGamesInHandMetricDesc = new MetricDescription<int>("# GIH", false, false);
    internal static readonly MetricDescription<float> WinRateInHandMetricDesc = new MetricDescription<float>("GIH WR", true, true, new PercentFormatter());
    internal static readonly MetricDescription<int> NumberOfGamesNotSeenMetricDesc = new MetricDescription<int>("# GNS", false, false);
    internal static readonly MetricDescription<float> WinRateNotSeenMetricDesc = new MetricDescription<float>("GNS WR", false, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WinRateImprovementWhenDrawnMetricDesc = new MetricDescription<float>("IWD", false, false, new PercentFormatter());

    internal static readonly MetricDescription<float> WUWinRateInHandMetricDesc = new MetricDescription<float>("WU", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WBWinRateInHandMetricDesc = new MetricDescription<float>("WB", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WRWinRateInHandMetricDesc = new MetricDescription<float>("WR", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WGWinRateInHandMetricDesc = new MetricDescription<float>("WG", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> UBWinRateInHandMetricDesc = new MetricDescription<float>("UB", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> URWinRateInHandMetricDesc = new MetricDescription<float>("UR", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> UGWinRateInHandMetricDesc = new MetricDescription<float>("UG", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> BRWinRateInHandMetricDesc = new MetricDescription<float>("BR", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> BGWinRateInHandMetricDesc = new MetricDescription<float>("BG", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> RGWinRateInHandMetricDesc = new MetricDescription<float>("RG", true, false, new PercentFormatter());
    internal static readonly CompositeMetricDescription ColorsWinRateInHandMetricDesc = new CompositeMetricDescription("GIH WR (2C)", true, false,
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

    internal static readonly MetricDescription<float> WUWinRateImprovementMetricDesc = new MetricDescription<float>("WU", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WBWinRateImprovementMetricDesc = new MetricDescription<float>("WB", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WRWinRateImprovementMetricDesc = new MetricDescription<float>("WR", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WGWinRateImprovementMetricDesc = new MetricDescription<float>("WG", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> UBWinRateImprovementMetricDesc = new MetricDescription<float>("UB", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> URWinRateImprovementMetricDesc = new MetricDescription<float>("UR", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> UGWinRateImprovementMetricDesc = new MetricDescription<float>("UG", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> BRWinRateImprovementMetricDesc = new MetricDescription<float>("BR", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> BGWinRateImprovementMetricDesc = new MetricDescription<float>("BG", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> RGWinRateImprovementMetricDesc = new MetricDescription<float>("RG", true, false, new PercentFormatter(true));
    internal static readonly CompositeMetricDescription ColorsWinRateImprovementMetricDesc = new CompositeMetricDescription("WR Delta (2C)", true, false,
                                                                                                                            WUWinRateImprovementMetricDesc,
                                                                                                                            WBWinRateImprovementMetricDesc,
                                                                                                                            WRWinRateImprovementMetricDesc,
                                                                                                                            WGWinRateImprovementMetricDesc,
                                                                                                                            UBWinRateImprovementMetricDesc,
                                                                                                                            URWinRateImprovementMetricDesc,
                                                                                                                            UGWinRateImprovementMetricDesc,
                                                                                                                            BRWinRateImprovementMetricDesc,
                                                                                                                            BGWinRateImprovementMetricDesc,
                                                                                                                            RGWinRateImprovementMetricDesc);
}
