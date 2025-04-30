using CardPile.CardData.Importance;

namespace CardPile.Crypt;

public class CardEntry : IBone
{
    internal CardEntry(string name, int cardId, Range range, ImportanceLevel importance)
    {
        Name = name;
        Range = range;
        Importance = importance;
        PrimaryCardId = cardId;
    }

    public string Name { get; }

    public Range Range { get; }

    public ImportanceLevel Importance { get; }

    public int PrimaryCardId { get; }
}
