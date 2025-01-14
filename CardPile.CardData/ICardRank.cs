using CardPile.CardData.Importance;

namespace CardPile.CardData;

public interface ICardRank
{
    public string Name { get; }

    public int Value { get; }

    public string TextValue { get; }

    public ImportanceLevel Importance { get; }
}
