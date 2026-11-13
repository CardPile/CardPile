using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class RawCardLluRatingFlags
{
    [JsonConstructor]
    internal RawCardLluRatingFlags
    (
        bool buildaround,
        bool synergy
    )
    {
        BuildAround = buildaround;
        Synergy = synergy;
    }

    internal bool BuildAround { get; init; }

    internal bool Synergy { get; init; }
}

internal class RawCardLluRating
{
    internal RawCardLluRating
    (
        string name,
        string tier,
        RawCardLluRatingFlags flags
    )
    {
        Name = name;
        Tier = tier;
        Flags = flags;
    }

    [JsonConstructor]
    internal RawCardLluRating
    (
        int card_id,
        string name,
        string expansion,
        string inclusion_type,
        string url,
        string url_back,
        string rarity,
        string color,
        string tier,
        int sort_key,
        string comment,
        List<string> types,
        string layout,
        double cmc,
        RawCardLluRatingFlags flags,
        string collector_number,
        string? trend,
        string trend_from,
        int history_count
    ) : this(name, tier, flags)
    {
    }

    internal string Name { get; init; }

    internal string Tier { get; init; }

    internal RawCardLluRatingFlags Flags { get; init; }
}
