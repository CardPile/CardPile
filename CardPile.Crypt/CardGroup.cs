using CardPile.CardData.Importance;

namespace CardPile.Crypt;

public class CardGroup : IBone
{
    internal CardGroup(string name, Range? range, ImportanceLevel importance)
    {
        Name = name;
        Range = range;
        Importance = importance;
    }

    public string Name { get; init; }

    public Range? Range { get; init; }

    public ImportanceLevel Importance { get; }

    public List<CardEntry> Cards { get; } = [];

    public List<CardGroup> Groups { get; } = [];

    Range EffectivRange()
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
            groupRange = CardPile.Crypt.Range.Intersect(groupRange, Range);
        }

        if(groupRange.IsEmpty())
        {
            return false;
        }

        return true;
    }
}
