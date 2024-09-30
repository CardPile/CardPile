namespace CardPile.CardData.Parameters;

public class Parameter : ICardDataSourceParameter
{
    protected Parameter(string name, ParameterType type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; init; }

    public ParameterType Type { get; init; }
}
