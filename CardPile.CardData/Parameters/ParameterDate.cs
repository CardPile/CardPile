namespace CardPile.CardData.Parameters;

public class ParameterDate : Parameter, ICardDataSourceParameterDate
{
    public ParameterDate(string name, DateTime date) : base(name, ParameterType.Date)
    {
        Value = date;
    }

    public DateTime Value { get; set; }
}
