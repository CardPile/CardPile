using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class RawWinData
{
    internal RawWinData
    (
        bool? isSummary,
        string? colorName,
        string? shortColorName,
        int? wins,
        int? games,
        Color colors
    )
    {
        IsSummary = isSummary;
        ColorName = colorName;
        ShortColorName = shortColorName;
        Wins = wins;
        Games = games;
        Colors = colors;
    }

    [JsonConstructor]
    internal RawWinData
    (
        bool? is_summary,
        string? color_name,
        string? short_name,
        int? wins,
        int? games
    ) : this(is_summary, color_name, short_name, wins, games, Utils.ParseColors(short_name))
    {}

    internal bool? IsSummary { get; init; }

    internal string? ColorName { get; init; }

    internal string? ShortColorName { get; init; }

    internal int? Wins { get; set; }

    internal int? Games { get; init; }

    internal Color Colors { get; init; }
}
