using CardPile.CardData.Importance;

namespace CardPile.Crypt;

public interface IBone
{
    public string Name { get; }

    public Range? Range { get; }

    public ImportanceLevel Importance { get; }

    public int Height { get; }

    public int Count { get; }

    public bool IsSatisfied { get; }

    public void ClearCount();

    public bool TryAddCard(int cardId);
}
