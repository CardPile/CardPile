using CardPile.CardData.Importance;

namespace CardPile.Crypt;

public interface IBone
{
    public string Name { get; }

    public Range? Range { get; }

    public ImportanceLevel Importance { get; }
}
