using CardPile.CardData.Parameters;

namespace CardPile.CardData;

public interface ICardDataSourceParameter
{
    public string Name { get; }

    public ParameterType Type { get; }
}
