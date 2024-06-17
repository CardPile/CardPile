namespace CardPile.CardData;

public interface ICardDataSourceParameter
{
    public string Name { get; }

    public CardDataSourceParameterType Type { get; }
}
