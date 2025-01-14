using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class RawCardData
{
    internal RawCardData
    (
        string name,
        int mtga_id,
        Color colors,
        Rarity rarity,
        string? url,
        int? seenCount,
        float? avgSeen,
        int? pickCount,
        float? avgPick,
        int? gameCount,
        float? playRate,
        float? winRate,
        int? openingHandGameCount,
        float? openingHandWinRate,
        int? drawnGameCount,
        float? drawnWinRate,
        int? everDrawnGameCount,
        float? everDrawnWinRate,
        int? neverDrawnGameCount,
        float? neverDrawnWinRate,
        float? drawnImprovementWinRate
    )
    {
        Name = name;
        ArenaCardId = mtga_id;
        Colors = colors;
        Rarity = rarity;
        Url = url;
        SeenCount = seenCount;
        AvgSeen = avgSeen;
        PickCount = pickCount;
        AvgPick = avgPick;
        GameCount = gameCount;
        PlayRate = playRate;
        WinRate = winRate;
        OpeningHandGameCount = openingHandGameCount;
        OpeningHandWinRate = openingHandWinRate;
        DrawnGameCount = drawnGameCount;
        DrawnWinRate = drawnWinRate;
        EverDrawnGameCount = everDrawnGameCount;
        EverDrawnWinRate = everDrawnWinRate;
        NeverDrawnGameCount = neverDrawnGameCount;
        NeverDrawnWinRate = neverDrawnWinRate;
        DrawnImprovementWinRate = drawnImprovementWinRate;

        SeenCountRanks = [];
        AvgSeenRanks = [];
        PickCountRanks = [];
        AvgPickRanks = [];
        GameCountRanks = [];
        PlayRateRanks = [];
        WinRateRanks = [];
        OpeningHandGameCountRanks = [];
        OpeningHandWinRateRanks = [];
        DrawnGameCountRanks = [];
        DrawnWinRateRanks = [];
        EverDrawnGameCountRanks = [];
        EverDrawnWinRateRanks = [];
        NeverDrawnGameCountRanks = [];
        NeverDrawnWinRateRanks = [];
        DrawnImprovementWinRateRanks = [];
    }

    [JsonConstructor]
    internal RawCardData
    (
        string name,
        int mtga_id,
        string? color,
        string? rarity,
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
    ) : this(name, mtga_id, Utils.ParseColors(color), Utils.ParseRarity(rarity), url, seen_count, avg_seen, pick_count, avg_pick, game_count, play_rate, win_rate, opening_hand_game_count, opening_hand_win_rate, drawn_game_count, drawn_win_rate, ever_drawn_game_count, ever_drawn_win_rate, never_drawn_game_count, never_drawn_win_rate, drawn_improvement_win_rate)
    {
    }

    internal string Name { get; init; }

    internal int ArenaCardId { get; init; }

    internal Color Colors { get; init; }

    internal Rarity Rarity { get; init; }

    internal string? Url { get; set; }

    internal int? SeenCount { get; init; }

    internal float? AvgSeen { get; init; }

    internal int? PickCount { get; init; }

    internal float? AvgPick { get; init; }

    internal int? GameCount { get; init; }

    internal float? PlayRate { get; init; }

    internal float? WinRate { get; init; }

    internal int? OpeningHandGameCount { get; init; }

    internal float? OpeningHandWinRate { get; init; }

    internal int? DrawnGameCount { get; init; }

    internal float? DrawnWinRate { get; init; }

    internal int? EverDrawnGameCount { get; init; }

    internal float? EverDrawnWinRate { get; init; }

    internal int? NeverDrawnGameCount { get; init; }

    internal float? NeverDrawnWinRate { get; init; }

    internal float? DrawnImprovementWinRate { get; init; }

    internal List<ICardRank> SeenCountRanks { get; init; }

    internal List<ICardRank> AvgSeenRanks { get; init; }

    internal List<ICardRank> PickCountRanks { get; init; }

    internal List<ICardRank> AvgPickRanks { get; init; }

    internal List<ICardRank> GameCountRanks { get; init; }

    internal List<ICardRank> PlayRateRanks { get; init; }

    internal List<ICardRank> WinRateRanks { get; init; }

    internal List<ICardRank> OpeningHandGameCountRanks { get; init; }

    internal List<ICardRank> OpeningHandWinRateRanks { get; init; }

    internal List<ICardRank> DrawnGameCountRanks { get; init; }

    internal List<ICardRank> DrawnWinRateRanks { get; init; }

    internal List<ICardRank> EverDrawnGameCountRanks { get; init; }

    internal List<ICardRank> EverDrawnWinRateRanks { get; init; }

    internal List<ICardRank> NeverDrawnGameCountRanks { get; init; }

    internal List<ICardRank> NeverDrawnWinRateRanks { get; init; }

    internal List<ICardRank> DrawnImprovementWinRateRanks { get; init; }
}
