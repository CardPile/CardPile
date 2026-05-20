using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class RawCardDEq
{
    internal RawCardDEq
    (
        string name,
        string? deqGrade,
        float? deq
    )
    {
        Name = name;
        DEqGrade = deqGrade != "N/A" ? deqGrade : null;
        DEq = deq;
    }

    [JsonConstructor]
    internal RawCardDEq
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
    ) : this(name, deq_grade, deq)
    {
    }

    internal string Name { get; init; }
    
    internal string? DEqGrade { get; init; }
    
    internal float? DEq { get; init; }
}
