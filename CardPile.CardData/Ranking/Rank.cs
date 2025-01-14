using CardPile.CardData.Importance;

namespace CardPile.CardData.Ranking;

public class Rank : ICardRank
{
    public Rank(string name, int value, ImportanceLevel importance)
    {
        Name = name;
        Value = value;
        Importance = importance;
    }

    public string Name { get; init; }

    public int Value { get; init; }

    public string TextValue
    {
        get
        {
            return (Value + 1).ToString();
        }
    }

    public ImportanceLevel Importance { get; init; }
}
