namespace CardPile.CardData;

public interface ICardDataSourceParameterDate : ICardDataSourceParameter
{
    public DateTime Value { get; set; }
}
