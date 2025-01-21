namespace CardPile.CardData.Parameters;

public class ParameterDate : Parameter, ICardDataSourceParameterDate
{
    public ParameterDate(string name, DateTime date) : base(name, ParameterType.Date)
    {
        this.date = date;
    }

    public DateTime Value
    { 
        get => date; 
        set => RaiseAndSetIfChanged(ref date, value);
    }

    public DateTime date;
}
