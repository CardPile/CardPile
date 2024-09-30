namespace CardPile.CardData.Parameters;

public class ParameterOptions : Parameter, ICardDataSourceParameterOptions
{
    public ParameterOptions(string name, List<string> options) : base(name, ParameterType.Options)
    {
        Options = options;
        Value = options.First();
    }

    public List<string> Options { get; init; }

    public string Value { get; set; }
}
