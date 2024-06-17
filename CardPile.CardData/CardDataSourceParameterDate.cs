namespace CardPile.CardData;

public class CardDataSourceParameterDate : CardDataSourceParameter, ICardDataSourceParameterDate
{
    public CardDataSourceParameterDate(string name, DateTime date) : base(name, CardDataSourceParameterType.Date)
    {
        Value = date;
    }

    public DateTime Value { get; set; }
}
