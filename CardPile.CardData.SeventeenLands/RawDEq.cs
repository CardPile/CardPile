using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class RawDEq
{
    internal RawDEq
    (
        string name,
        Color colors,
        Rarity rarity,
        string? url,
        string? deqGrade,        
        float? deq,
        float? marginalWinRate,
        float? pickEquity,
        float? adjustment,
        float? normalizedPickRate,
        float? percentTop,
        float? percentGp
    )
    {
        Name = name;
        Colors = colors;
        Rarity = rarity;
        Url = url;

        DEqGrade = deqGrade != "N/A" ? deqGrade : null;
        DEq = deq;
    }

    [JsonConstructor]
    internal RawDEq
    (
        string deq_grade,
        string name,
        string color,
        string rarity,
        float? deq,
        float? mwr,
        float? pick_equity,
        float? adj,
        float? npr,
        float? pct_top,
        float? pct_gp,
        string? image_url
    ) : this(name, Utils.ParseColors(color), Utils.ParseRarity(rarity), image_url, deq_grade, deq, mwr, pick_equity, adj, npr, pct_top, pct_gp)
    {
    }

    internal string Name { get; init; }
    
    internal Color Colors { get; init; }

    internal Rarity Rarity { get; init; }

    internal string? Url { get; init; }

    internal string? DEqGrade { get; init; }
    
    internal float? DEq { get; init; }
}
