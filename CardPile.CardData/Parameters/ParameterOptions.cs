namespace CardPile.CardData.Parameters;

public class ParameterOptions : Parameter, ICardDataSourceParameterOptions
{
    public ParameterOptions(string name, List<string> options, string? defaultValue = null) : base(name, ParameterType.Options)
    {
        if (defaultValue != null && !options.Contains(defaultValue))
        {
            throw new ArgumentException($"Invalid default option {defaultValue}", nameof(defaultValue));
        }

        possibleOptions = options;
        selectedOption = defaultValue ?? options.First();
    }

    public List<string> Options
    {
        get => possibleOptions;
        set => RaiseAndSetIfChanged(ref possibleOptions, value);
    }

    public string Value
    { 
        get => selectedOption;
        set => RaiseAndSetIfChanged(ref selectedOption, value);
    }

    private List<string> possibleOptions;
    private string selectedOption;
}
