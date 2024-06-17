namespace CardPile.CardData;

public class CardDataSourceParameterOptions : CardDataSourceParameter, ICardDataSourceParameterOptions
{
    public CardDataSourceParameterOptions(string name, List<string> options) : base(name, CardDataSourceParameterType.Options)
    {
        Options = options;
        Value = options.First();
    }

    public List<string> Options { get; init; }

    public string Value { get; set; }
}
