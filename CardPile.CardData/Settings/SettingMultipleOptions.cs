namespace CardPile.CardData.Settings;

public class SettingMultipleOptions : Setting, ICardDataSourceSettingMultipleOptions
{
    public SettingMultipleOptions(string name, List<ICardDataSourceSettingOption> options) : base(name, SettingType.MultipleOptions)
    {
        Options = options;
    }

    public List<ICardDataSourceSettingOption> Options { get; init; }
}
