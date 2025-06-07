using CardPile.CardData.Importance;
using CardPile.CardInfo;
using NLog;
using Tomlet.Models;

namespace CardPile.Crypt;

public class CardEntry : IBone
{
    internal CardEntry(CardGroup parent, string name, List<int> cardIds, Range? range, ImportanceLevel importance)
    {
        Parent = parent;
        Name = name;
        Range = range;
        Importance = importance;
        CardIds = cardIds;
        Count = 0;
    }

    public CardGroup Parent { get; init; }

    public string Name { get; }

    public Range? Range { get; }

    public ImportanceLevel Importance { get; }

    public int Height { get => 0; }

    public int Count { get; private set; }

    public bool IsSatisfied { get => (Range == null || Range.Contains(Count)); }

    public List<int> CardIds { get; }

    public int PrimaryCardId { get => CardIds.First(); }

    public void ClearCount()
    {
        Count = 0;
    }

    public bool TryAddCard(int cardId)
    {
        if (!CanAddCard(cardId))
        {
            return false;
        }

        Count++;

        return true;
    }

    public bool CanAddCard(int cardId)
    {
        if(!CardIds.Contains(cardId))
        {
            return false;
        }

        if (Range != null && Range.To < Count + 1)
        {
            return false;
        }

        var parent = Parent;
        while (parent != null)
        {
            if(parent.Range != null && parent.Range.To < Count + 1)
            {
                return false;
            }
            parent = parent.Parent;
        }

        return true;
    }

    internal static CardEntry? TryLoad(TomlArray card, CardGroup parent, string set)
    {
        if (card.Count < 2 && 3 < card.Count)
        {
            logger.Error("Error parsing card entry in skeleton. Too many entries in card description {card}", card);
            return null;
        }

        var cardName = card.ArrayValues[0].StringValue.Trim();
        if (string.IsNullOrEmpty(cardName))
        {
            logger.Error("Error parsing card entry in skeleton. Invalid card name {cardName}", cardName);
            return null;
        }

        var cardIds = Arena.GetCardIdsFromNameAndExpansion(cardName, set);
        if (cardIds.Count == 0)
        {
            logger.Error("Error parsing card entry in skeleton. Unknown card name {cardName}", cardName);
            return null;
        }

        var range = (Range?)null;
        var rangeString = card.ArrayValues[1].StringValue;
        if (!string.IsNullOrWhiteSpace(rangeString))
        {
            range = Range.TryParse(rangeString);
            if (range == null)
            {
                logger.Error("Error parsing card entry in skeleton. Invalid range {rangeString}", rangeString);
                return null;
            }
        }

        var importance = ImportanceLevel.Regular;
        if (card.Count == 3)
        {
            var importanceString = card.ArrayValues[2].StringValue;
            if (!Enum.TryParse(importanceString, true, out importance))
            {
                logger.Error("Error parsing card entry in skeleton. Invalid importance {importanceString}", importanceString);
                return null;
            }
        }

        return new CardEntry(parent, cardName, cardIds, range, importance);
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
