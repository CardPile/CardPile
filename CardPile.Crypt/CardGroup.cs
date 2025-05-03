using CardPile.CardData.Importance;
using NLog;
using Tomlet.Models;

namespace CardPile.Crypt;

public class CardGroup : IBone
{
    internal CardGroup(string name, Range? range, ImportanceLevel importance, List<CardEntry> cards, List<CardGroup> groups)
    {
        Name = name;
        Range = range;
        Importance = importance;
        Cards = cards;
        Groups = groups;

        Height = Cards.Count > 0 ? 1 : 0;
        foreach(var group in Groups)
        {
            Height = Math.Max(Height, group.Height + 1);
        }
    }

    public string Name { get; init; }

    public Range? Range { get; init; }

    public ImportanceLevel Importance { get; }

    public int Height { get; }

    public int Count
    {
        get
        {
            return Cards.Sum(c => c.Count) + Groups.Sum(g => g.Count);
        }
    }

    public bool IsSatisfied { get => (Range == null || Range.Contains(Count)) && Cards.All(x => x.IsSatisfied) && Groups.All(x => x.IsSatisfied); }

    public List<CardEntry> Cards { get; }

    public List<CardGroup> Groups { get; }

    public void ClearCount()
    {
        foreach(var card in Cards)
        {
            card.ClearCount();
        }
        foreach(var group in Groups)
        {
            group.ClearCount();
        }
    }

    public bool TryAddCard(int cardId)
    {
        if(Range != null && Range.To < Count + 1)
        {
            return false;
        }

        foreach(var card in Cards)
        {
            if(card.TryAddCard(cardId))
            {
                return true;
            }
        }

        foreach (var group in Groups)
        {
            if (group.TryAddCard(cardId))
            {
                return true;
            }
        }

        return false;
    }

    internal static CardGroup? TryLoad(string name, TomlTable table, string set)
    {
        Range? totalRange = null;
        if (table.TryGetValue("total", out var totalValue))
        {
            var totalRangeString = totalValue.StringValue;
            var parsedTotalRange = Range.TryParse(totalRangeString);
            if (parsedTotalRange == null)
            {
                logger.Error("Error parsing card group in skeleton. Invalid total range {totalRangeString}", totalRangeString);
                return null;
            }

            totalRange = parsedTotalRange;
        }

        var importance = ImportanceLevel.Regular;
        if (table.TryGetValue("importance", out var importanceValue))
        {
            var importanceString = importanceValue.StringValue;
            if (!Enum.TryParse(importanceString, true, out importance))
            {
                logger.Error("Error parsing card group in skeleton. Invalid importance {importanceString}", importanceString);
                return null;
            }
        }

        var cards = new List<CardEntry>();
        if (table.TryGetValue("cards", out var cardsArrayValue) && cardsArrayValue is TomlArray cardsArray)
        {
            foreach (var cardArrayEntry in cardsArray)
            {
                if (cardArrayEntry is not TomlArray card)
                {
                    logger.Error("Error parsing card group in skeleton. Card entry is not an array {cardEntry}", cardArrayEntry);
                    return null;
                }

                var parsedCard = CardEntry.TryLoad(card, set);
                if (parsedCard == null)
                {
                    logger.Error("Error parsing card group in skeleton. Invalid card entry {card}", card);
                    return null;
                }

                cards.Add(parsedCard);
            }

            cards.Sort((lhs, rhs) => -Comparer<ImportanceLevel>.Default.Compare(lhs.Importance, rhs.Importance));
        }

        var groups = new List<CardGroup>();
        foreach (var subtableEntry in table.Where(kv => kv.Value is TomlTable))
        {
            if (subtableEntry.Value is not TomlTable subtable)
            {
                logger.Error("Error parsing card group in skeleton. Subtable is not a table {subtableEntry}", subtableEntry);
                return null;
            }

            var cardGroup = TryLoad(subtableEntry.Key, subtable, set);
            if (cardGroup == null)
            {
                logger.Error("Error parsing card group in skeleton. Invalid subtable {subtable}", subtable);
                return null;
            }

            if (cardGroup.Groups.Count == 0 && cardGroup.Cards.Count == 0)
            {
                logger.Warn("Card group {cardGroup} is empty.", table);
                continue;
            }

            groups.Add(cardGroup);
        }

        return new CardGroup(name, totalRange, importance, cards, groups);
    }


    internal Range EffectivRange()
    {
        Range result = new();
        foreach (var card in Cards)
        {
            result.Extend(card.Range);
        }

        foreach (var group in Groups)
        {
            result.Extend(group.EffectivRange());
        }

        return result;
    }

    internal bool CanBeSatisfied()
    {
        foreach(var group in Groups)
        {
            if (!group.CanBeSatisfied())
            {
                return false;
            }
        }

        var groupRange = EffectivRange();
        if (Range != null)
        {
            groupRange = Range.Intersect(groupRange, Range);
        }

        if(groupRange.Empty)
        {
            return false;
        }

        return true;
    }

    internal List<IBone> GetAllChildBones()
    {
        var result = new List<IBone>();
        foreach (var group in Groups)
        {
            result.Add(group);
            result.AddRange(group.GetAllChildBones());
        }

        foreach (var card in Cards)
        {
            result.Add(card);
        }

        return result;
    }

    internal List<CardEntry> GetAllCardEntries()
    {
        var result = new List<CardEntry>();

        foreach (var group in Groups)
        {
            result.AddRange(group.GetAllCardEntries());
        }

        result.AddRange(Cards);

        return result;
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
