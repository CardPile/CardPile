namespace CardPile.CardData;

public class CardDataSourceParameter : ICardDataSourceParameter
{
    protected CardDataSourceParameter(string name, CardDataSourceParameterType type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; init; }

    public CardDataSourceParameterType Type { get; init; }
}
