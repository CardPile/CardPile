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
        ColorPairsWinRateInHandMetricDesc.NewMetric(
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
        ColorPairsWinRateImprovementMetricDesc.NewMetric(
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
        ColorTriplesWinRateInHandMetricDesc.NewMetric(
            WUBWinRateInHandMetricDesc.NewMetric(),
            WURWinRateInHandMetricDesc.NewMetric(),
            WUGWinRateInHandMetricDesc.NewMetric(),
            WBRWinRateInHandMetricDesc.NewMetric(),
            WBGWinRateInHandMetricDesc.NewMetric(),
            WRGWinRateInHandMetricDesc.NewMetric(),
            UBRWinRateInHandMetricDesc.NewMetric(),
            UBGWinRateInHandMetricDesc.NewMetric(),
            URGWinRateInHandMetricDesc.NewMetric(),
            BRGWinRateInHandMetricDesc.NewMetric()
        ),
        ColorTriplesWinRateImprovementMetricDesc.NewMetric(
            WUBWinRateImprovementMetricDesc.NewMetric(),
            WURWinRateImprovementMetricDesc.NewMetric(),
            WUGWinRateImprovementMetricDesc.NewMetric(),
            WBRWinRateImprovementMetricDesc.NewMetric(),
            WBGWinRateImprovementMetricDesc.NewMetric(),
            WRGWinRateImprovementMetricDesc.NewMetric(),
            UBRWinRateImprovementMetricDesc.NewMetric(),
            UBGWinRateImprovementMetricDesc.NewMetric(),
            URGWinRateImprovementMetricDesc.NewMetric(),
            BRGWinRateImprovementMetricDesc.NewMetric()
        ),
        NumberOfGamesNotSeenMetricDesc.NewMetric(),
        WinRateNotSeenMetricDesc.NewMetric(),
        WinRateImprovementWhenDrawnMetricDesc.NewMetric(),
        DEqMetricDesc.NewMetric(),
        DEqGradeMetricDesc.NewMetric()
        )
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
        ICardMetric colorPairsWinRateInHandMetric,
        ICardMetric colorPairsWinRateImprovementMetric,
        ICardMetric colorTriplesWinRateInHandMetric,
        ICardMetric colorTriplesWinRateImprovementMetric,
        ICardMetric numberOfGamesNotSeenMetric,
        ICardMetric winRateNotSeenMetric,
        ICardMetric winRateImprovementWhenDrawnMetric,
        ICardMetric deqMetric,
        ICardMetric deqGradeMetric
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
            colorPairsWinRateInHandMetric,
            colorPairsWinRateImprovementMetric,
            colorTriplesWinRateInHandMetric,
            colorTriplesWinRateImprovementMetric,
            numberOfGamesNotSeenMetric,
            winRateNotSeenMetric,
            winRateImprovementWhenDrawnMetric,
            deqMetric,
            deqGradeMetric,
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
                ColorPairsWinRateInHandMetricDesc,
                ColorPairsWinRateImprovementMetricDesc,
                ColorTriplesWinRateInHandMetricDesc,
                ColorTriplesWinRateImprovementMetricDesc,
                NumberOfGamesNotSeenMetricDesc,
                WinRateNotSeenMetricDesc,
                WinRateImprovementWhenDrawnMetricDesc,
                DEqMetricDesc,
                DEqGradeMetricDesc,
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

    internal static readonly MetricDescription<float> WUWinRateInHandMetricDesc = new MetricDescription<float>("{W}{U}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WBWinRateInHandMetricDesc = new MetricDescription<float>("{W}{B}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WRWinRateInHandMetricDesc = new MetricDescription<float>("{W}{R}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WGWinRateInHandMetricDesc = new MetricDescription<float>("{W}{G}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> UBWinRateInHandMetricDesc = new MetricDescription<float>("{U}{B}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> URWinRateInHandMetricDesc = new MetricDescription<float>("{U}{R}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> UGWinRateInHandMetricDesc = new MetricDescription<float>("{U}{G}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> BRWinRateInHandMetricDesc = new MetricDescription<float>("{B}{R}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> BGWinRateInHandMetricDesc = new MetricDescription<float>("{B}{G}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> RGWinRateInHandMetricDesc = new MetricDescription<float>("{R}{G}", true, false, new PercentFormatter());
    internal static readonly CompositeMetricDescription ColorPairsWinRateInHandMetricDesc = new CompositeMetricDescription("GIH WR (2C)", true, false,
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

    internal static readonly MetricDescription<float> WUWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{U}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WBWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{B}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WRWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{R}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WGWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{G}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> UBWinRateImprovementMetricDesc = new MetricDescription<float>("{U}{B}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> URWinRateImprovementMetricDesc = new MetricDescription<float>("{U}{R}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> UGWinRateImprovementMetricDesc = new MetricDescription<float>("{U}{G}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> BRWinRateImprovementMetricDesc = new MetricDescription<float>("{B}{R}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> BGWinRateImprovementMetricDesc = new MetricDescription<float>("{B}{G}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> RGWinRateImprovementMetricDesc = new MetricDescription<float>("{R}{G}", true, false, new PercentFormatter(true));
    internal static readonly CompositeMetricDescription ColorPairsWinRateImprovementMetricDesc = new CompositeMetricDescription("WR Delta (2C)", true, false,
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

    internal static readonly MetricDescription<float> WUBWinRateInHandMetricDesc = new MetricDescription<float>("{W}{U}{B}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WURWinRateInHandMetricDesc = new MetricDescription<float>("{W}{U}{R}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WUGWinRateInHandMetricDesc = new MetricDescription<float>("{W}{U}{G}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WBRWinRateInHandMetricDesc = new MetricDescription<float>("{W}{B}{R}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WBGWinRateInHandMetricDesc = new MetricDescription<float>("{W}{B}{G}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> WRGWinRateInHandMetricDesc = new MetricDescription<float>("{W}{R}{G}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> UBRWinRateInHandMetricDesc = new MetricDescription<float>("{U}{B}{R}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> UBGWinRateInHandMetricDesc = new MetricDescription<float>("{U}{B}{G}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> URGWinRateInHandMetricDesc = new MetricDescription<float>("{U}{R}{G}", true, false, new PercentFormatter());
    internal static readonly MetricDescription<float> BRGWinRateInHandMetricDesc = new MetricDescription<float>("{B}{R}{G}", true, false, new PercentFormatter());
    internal static readonly CompositeMetricDescription ColorTriplesWinRateInHandMetricDesc = new CompositeMetricDescription("GIH WR (3C)", true, false,
                                                                                                                             WUBWinRateInHandMetricDesc,
                                                                                                                             WURWinRateInHandMetricDesc,
                                                                                                                             WUGWinRateInHandMetricDesc,
                                                                                                                             WBRWinRateInHandMetricDesc,
                                                                                                                             WBGWinRateInHandMetricDesc,
                                                                                                                             WRGWinRateInHandMetricDesc,
                                                                                                                             UBRWinRateInHandMetricDesc,
                                                                                                                             UBGWinRateInHandMetricDesc,
                                                                                                                             URGWinRateInHandMetricDesc,
                                                                                                                             BRGWinRateInHandMetricDesc);

    internal static readonly MetricDescription<float> WUBWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{U}{B}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WURWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{U}{R}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WUGWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{U}{G}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WBRWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{B}{R}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WBGWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{B}{G}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> WRGWinRateImprovementMetricDesc = new MetricDescription<float>("{W}{R}{G}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> UBRWinRateImprovementMetricDesc = new MetricDescription<float>("{U}{B}{R}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> UBGWinRateImprovementMetricDesc = new MetricDescription<float>("{U}{B}{G}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> URGWinRateImprovementMetricDesc = new MetricDescription<float>("{U}{R}{G}", true, false, new PercentFormatter(true));
    internal static readonly MetricDescription<float> BRGWinRateImprovementMetricDesc = new MetricDescription<float>("{B}{R}{G}", true, false, new PercentFormatter(true));
    internal static readonly CompositeMetricDescription ColorTriplesWinRateImprovementMetricDesc = new CompositeMetricDescription("WR Delta (3C)", true, false,
                                                                                                                                  WUBWinRateImprovementMetricDesc,
                                                                                                                                  WURWinRateImprovementMetricDesc,
                                                                                                                                  WUGWinRateImprovementMetricDesc,
                                                                                                                                  WBRWinRateImprovementMetricDesc,
                                                                                                                                  WBGWinRateImprovementMetricDesc,
                                                                                                                                  WRGWinRateImprovementMetricDesc,
                                                                                                                                  UBRWinRateImprovementMetricDesc,
                                                                                                                                  UBGWinRateImprovementMetricDesc,
                                                                                                                                  URGWinRateImprovementMetricDesc,
                                                                                                                                  BRGWinRateImprovementMetricDesc);

    internal static readonly MetricDescription<float> DEqMetricDesc = new MetricDescription<float>("DEq", true, false, new PercentFormatter(true));
    internal static readonly LetterGradeMetricDescription DEqGradeMetricDesc = new LetterGradeMetricDescription("DEq grade", true, false);
}
