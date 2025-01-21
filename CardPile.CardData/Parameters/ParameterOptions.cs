namespace CardPile.CardData.Parameters;

public class ParameterOptions : Parameter, ICardDataSourceParameterOptions
{
    public ParameterOptions(string name, List<string> options) : base(name, ParameterType.Options)
    {
        Options = options;
        option = options.First();
    }

    public List<string> Options { get; init; }

    public string Value
    { 
        get => option;
        set => RaiseAndSetIfChanged(ref option, value);
    }

    public string option;
}
