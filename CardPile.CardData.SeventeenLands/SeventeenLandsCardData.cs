using CardPile.CardData.Formatting;
using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class SeventeenLandsCardData : ICardData
{
    internal SeventeenLandsCardData(string name, int mtga_id, List<Color> colors) : this(name, mtga_id, colors, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)
    {}

    internal SeventeenLandsCardData(string name, int mtga_id, List<Color> colors, string? url) : this(name, mtga_id, colors, url, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)
    { }

    internal SeventeenLandsCardData
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
        Metrics =
        [
            SeenMetricDesc.NewMetric(seen_count),
            AverageLastSeenAtMetricDesc.NewMetric(avg_seen),
            PickedMetricDesc.NewMetric(pick_count),
            AveragePickedAtMetricDesc.NewMetric(avg_pick),
            NumberOfGamesPlayedMetricDesc.NewMetric(game_count),
            PlayRateMetricDesc.NewMetric(play_rate),
            WinRateWhenMaindeckedMetricDesc.NewMetric(win_rate),
            NumberOfGamesInOpeningHandMetricDesc.NewMetric(opening_hand_game_count),
            WinRateInOpeningHandMetricDesc.NewMetric(opening_hand_win_rate),
            NumberOfGamesDrawnTurn1OrLaterMetricDesc.NewMetric(drawn_game_count),
            WinRateWhenDrawnTurn1OrLaterMetricDesc.NewMetric(drawn_win_rate),
            NumberOfGamesInHandMetricDesc.NewMetric(ever_drawn_game_count),
            WinRateInHandMetricDesc.NewMetric(ever_drawn_win_rate),
            NumberOfGamesNotSeenMetricDesc.NewMetric(never_drawn_game_count),
            WinRateNotSeenMetricDesc.NewMetric(never_drawn_win_rate),
            WinRateImprovementWhenDrawnMetricDesc.NewMetric(drawn_improvement_win_rate),
        ];
    }

    [JsonConstructor]
    internal SeventeenLandsCardData
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
    ) : this(name, mtga_id, [], url, seen_count, avg_seen, pick_count, avg_pick, game_count, play_rate, win_rate, opening_hand_game_count, opening_hand_win_rate, drawn_game_count, drawn_win_rate, ever_drawn_game_count, ever_drawn_win_rate, never_drawn_game_count, never_drawn_win_rate, drawn_improvement_win_rate)
    {
        Colors = ParseColors(color);
    }

    public string Name { get; init; }

    public int ArenaCardId { get; init; }

    public List<Color> Colors { get; init; }

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

    private static readonly CardMetricDescription<int>      SeenMetricDesc = new CardMetricDescription<int>("# Seen", false, false);
    private static readonly CardMetricDescription<float>    AverageLastSeenAtMetricDesc = new CardMetricDescription<float>("ALSA", true, false, new CardMetricDecimalFormatter());
    private static readonly CardMetricDescription<int>      PickedMetricDesc = new CardMetricDescription<int>("# Picked", false, false);
    private static readonly CardMetricDescription<float>    AveragePickedAtMetricDesc = new CardMetricDescription<float>("ATA", true, false, new CardMetricDecimalFormatter());
    private static readonly CardMetricDescription<int>      NumberOfGamesPlayedMetricDesc = new CardMetricDescription<int>("# GP", false, false);
    private static readonly CardMetricDescription<float>    PlayRateMetricDesc = new CardMetricDescription<float>("% GP", false, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float>    WinRateWhenMaindeckedMetricDesc = new CardMetricDescription<float>("GP WR", false, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<int>      NumberOfGamesInOpeningHandMetricDesc = new CardMetricDescription<int>("# OH", false, false);
    private static readonly CardMetricDescription<float>    WinRateInOpeningHandMetricDesc = new CardMetricDescription<float>("OH WR", false, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<int>      NumberOfGamesDrawnTurn1OrLaterMetricDesc = new CardMetricDescription<int>("# GD", false, false);
    private static readonly CardMetricDescription<float>    WinRateWhenDrawnTurn1OrLaterMetricDesc = new CardMetricDescription<float>("GD WR", false, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<int>      NumberOfGamesInHandMetricDesc = new CardMetricDescription<int>("# GIH", false, false);
    private static readonly CardMetricDescription<float>    WinRateInHandMetricDesc = new CardMetricDescription<float>("GIH WR", true, true, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<int>      NumberOfGamesNotSeenMetricDesc = new CardMetricDescription<int>("# GNS", false, false);
    private static readonly CardMetricDescription<float>    WinRateNotSeenMetricDesc = new CardMetricDescription<float>("GNS WR", false, false, new CardMetricPercentFormatter());
    private static readonly CardMetricDescription<float>    WinRateImprovementWhenDrawnMetricDesc = new CardMetricDescription<float>("IWD", false, false, new CardMetricPercentFormatter());
}
