using CardPile.CardData.Formatting;
using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class SeventeenLandsRawCardData : ICardData
{
    internal SeventeenLandsRawCardData(string name, int mtga_id, List<Color> colors) : this(name, mtga_id, colors, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)
    { }

    internal SeventeenLandsRawCardData(string name, int mtga_id, List<Color> colors, string? url) : this(name, mtga_id, colors, url, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)
    { }

    internal SeventeenLandsRawCardData
    (
        string name,
        int mtga_id,
        List<Color> colors,
        string? url,
        int? seen_count,                    // # Seen
        float? avg_seen,                    // ALSA
        int? pick_count,                    // # Picked
        float? avg_pick,                    // ATA
        int? game_count,                    // # GP
        float? play_rate,                   // % GP
        float? win_rate,                    // GP WR
        int? opening_hand_game_count,       // # OH
        float? opening_hand_win_rate,       // OH WR
        int? drawn_game_count,              // # GD
        float? drawn_win_rate,              // GD WR
        int? ever_drawn_game_count,         // # GIH
        float? ever_drawn_win_rate,         // GIH WR
        int? never_drawn_game_count,        // # GNS
        float? never_drawn_win_rate,        // GNS WR
        float? drawn_improvement_win_rate   // IWD,
    )
    {
        Name = name;
        ArenaCardId = mtga_id;
        Colors = colors;
        Url = url;

        SeenMetric = SeenMetricDesc.NewMetric(seen_count);
        AverageLastSeenAtMetric = AverageLastSeenAtMetricDesc.NewMetric(avg_seen);
        PickedMetric = PickedMetricDesc.NewMetric(pick_count);
        AveragePickedAtMetric = AveragePickedAtMetricDesc.NewMetric(avg_pick);
        NumberOfGamesPlayedMetric = NumberOfGamesPlayedMetricDesc.NewMetric(game_count);
        PlayRateMetric = PlayRateMetricDesc.NewMetric(play_rate);
        WinRateWhenMaindeckedMetric = WinRateWhenMaindeckedMetricDesc.NewMetric(win_rate);
        NumberOfGamesInOpeningHandMetric = NumberOfGamesInOpeningHandMetricDesc.NewMetric(opening_hand_game_count);
        WinRateInOpeningHandMetric = WinRateInOpeningHandMetricDesc.NewMetric(opening_hand_win_rate);
        NumberOfGamesDrawnTurn1OrLaterMetric = NumberOfGamesDrawnTurn1OrLaterMetricDesc.NewMetric(drawn_game_count);
        WinRateWhenDrawnTurn1OrLaterMetric = WinRateWhenDrawnTurn1OrLaterMetricDesc.NewMetric(drawn_win_rate);
        NumberOfGamesInHandMetric = NumberOfGamesInHandMetricDesc.NewMetric(ever_drawn_game_count);
        WinRateInHandMetric = WinRateInHandMetricDesc.NewMetric(ever_drawn_win_rate);
        NumberOfGamesNotSeenMetric = NumberOfGamesNotSeenMetricDesc.NewMetric(never_drawn_game_count);
        WinRateNotSeenMetric = WinRateNotSeenMetricDesc.NewMetric(never_drawn_win_rate);
        WinRateImprovementWhenDrawnMetric = WinRateImprovementWhenDrawnMetricDesc.NewMetric(drawn_improvement_win_rate);

        Metrics =
        [
            SeenMetric,
            AverageLastSeenAtMetric,
            PickedMetric,
            AveragePickedAtMetric,
            NumberOfGamesPlayedMetric,
            PlayRateMetric,
            WinRateWhenMaindeckedMetric,
            NumberOfGamesInOpeningHandMetric,
            WinRateInOpeningHandMetric,
            NumberOfGamesDrawnTurn1OrLaterMetric,
            WinRateWhenDrawnTurn1OrLaterMetric,
            NumberOfGamesInHandMetric,
            WinRateInHandMetric,
            NumberOfGamesNotSeenMetric,
            WinRateNotSeenMetric,
            WinRateImprovementWhenDrawnMetric,
        ];
    }

    [JsonConstructor]
    internal SeventeenLandsRawCardData
    (
        string name,
        int mtga_id,
        string? color,
        string? url,
        int? seen_count,                    // # Seen
        float? avg_seen,                    // ALSA
        int? pick_count,                    // # Picked
        float? avg_pick,                    // ATA
        int? game_count,                    // # GP
        float? play_rate,                   // % GP
        float? win_rate,                    // GP WR
        int? opening_hand_game_count,       // # OH
        float? opening_hand_win_rate,       // OH WR
        int? drawn_game_count,              // # GD
        float? drawn_win_rate,              // GD WR
        int? ever_drawn_game_count,         // # GIH
        float? ever_drawn_win_rate,         // GIH WR
        int? never_drawn_game_count,        // # GNS
        float? never_drawn_win_rate,        // GNS WR
        float? drawn_improvement_win_rate   // IWD,
    ) : this(name, mtga_id, ParseColors(color), url, seen_count, avg_seen, pick_count, avg_pick, game_count, play_rate, win_rate, opening_hand_game_count, opening_hand_win_rate, drawn_game_count, drawn_win_rate, ever_drawn_game_count, ever_drawn_win_rate, never_drawn_game_count, never_drawn_win_rate, drawn_improvement_win_rate)
    {
    }

    public string Name { get; init; }

    public int ArenaCardId { get; init; }

    public List<Color> Colors { get; init; }

    public string? Url { get; set; }

    public List<ICardMetric> Metrics { get; init; }

    internal CardMetric<int> SeenMetric { get; init; }
    internal CardMetric<float> AverageLastSeenAtMetric { get; init; }
    internal CardMetric<int> PickedMetric { get; init; }
    internal CardMetric<float> AveragePickedAtMetric { get; init; }
    internal CardMetric<int> NumberOfGamesPlayedMetric { get; init; }
    internal CardMetric<float> PlayRateMetric { get; init; }
    internal CardMetric<float> WinRateWhenMaindeckedMetric { get; init; }
    internal CardMetric<int> NumberOfGamesInOpeningHandMetric { get; init; }
    internal CardMetric<float> WinRateInOpeningHandMetric { get; init; }
    internal CardMetric<int> NumberOfGamesDrawnTurn1OrLaterMetric { get; init; }
    internal CardMetric<float> WinRateWhenDrawnTurn1OrLaterMetric { get; init; }
    internal CardMetric<int> NumberOfGamesInHandMetric { get; init; }
    internal CardMetric<float> WinRateInHandMetric { get; init; }
    internal CardMetric<int> NumberOfGamesNotSeenMetric { get; init; }
    internal CardMetric<float> WinRateNotSeenMetric { get; init; }
    internal CardMetric<float> WinRateImprovementWhenDrawnMetric { get; init; }

    internal static readonly CardMetricDescription<int> SeenMetricDesc = new CardMetricDescription<int>("# Seen", false, false);
    internal static readonly CardMetricDescription<float> AverageLastSeenAtMetricDesc = new CardMetricDescription<float>("ALSA", true, false, new CardMetricDecimalFormatter());
    internal static readonly CardMetricDescription<int> PickedMetricDesc = new CardMetricDescription<int>("# Picked", false, false);
    internal static readonly CardMetricDescription<float> AveragePickedAtMetricDesc = new CardMetricDescription<float>("ATA", true, false, new CardMetricDecimalFormatter());
    internal static readonly CardMetricDescription<int> NumberOfGamesPlayedMetricDesc = new CardMetricDescription<int>("# GP", false, false);
    internal static readonly CardMetricDescription<float> PlayRateMetricDesc = new CardMetricDescription<float>("% GP", false, false, new CardMetricPercentFormatter());
    internal static readonly CardMetricDescription<float> WinRateWhenMaindeckedMetricDesc = new CardMetricDescription<float>("GP WR", false, false, new CardMetricPercentFormatter());
    internal static readonly CardMetricDescription<int> NumberOfGamesInOpeningHandMetricDesc = new CardMetricDescription<int>("# OH", false, false);
    internal static readonly CardMetricDescription<float> WinRateInOpeningHandMetricDesc = new CardMetricDescription<float>("OH WR", false, false, new CardMetricPercentFormatter());
    internal static readonly CardMetricDescription<int> NumberOfGamesDrawnTurn1OrLaterMetricDesc = new CardMetricDescription<int>("# GD", false, false);
    internal static readonly CardMetricDescription<float> WinRateWhenDrawnTurn1OrLaterMetricDesc = new CardMetricDescription<float>("GD WR", false, false, new CardMetricPercentFormatter());
    internal static readonly CardMetricDescription<int> NumberOfGamesInHandMetricDesc = new CardMetricDescription<int>("# GIH", false, false);
    internal static readonly CardMetricDescription<float> WinRateInHandMetricDesc = new CardMetricDescription<float>("GIH WR", true, true, new CardMetricPercentFormatter());
    internal static readonly CardMetricDescription<int> NumberOfGamesNotSeenMetricDesc = new CardMetricDescription<int>("# GNS", false, false);
    internal static readonly CardMetricDescription<float> WinRateNotSeenMetricDesc = new CardMetricDescription<float>("GNS WR", false, false, new CardMetricPercentFormatter());
    internal static readonly CardMetricDescription<float> WinRateImprovementWhenDrawnMetricDesc = new CardMetricDescription<float>("IWD", false, false, new CardMetricPercentFormatter());

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
                NumberOfGamesNotSeenMetricDesc,
                WinRateNotSeenMetricDesc,
                WinRateImprovementWhenDrawnMetricDesc,
            ];
        }
    }

    private static List<Color> ParseColors(string? colors)
    {
        if (colors == null)
        {
            return [];
        }

        var result = new List<Color>();
        foreach (var color in colors)
        {
            if(color == 'W')
            {
                result.Add(Color.White);
            }
            else if(color == 'U')
            {
                result.Add(Color.Blue);
            }
            else if (color == 'B')
            {
                result.Add(Color.Black);
            }
            else if (color == 'R')
            {
                result.Add(Color.Red);
            }
            else if (color == 'G')
            {
                result.Add(Color.Green);
            }
        }

        return result;
    }
}
