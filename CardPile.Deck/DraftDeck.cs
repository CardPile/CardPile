using CardPile.CardData;

namespace CardPile.Deck;

public class DraftDeck
{
    public DraftDeck(List<ICardData> cards)
    {
        Cards = cards;
    }
    
    public List<ICardData> Cards { get; init; }
}