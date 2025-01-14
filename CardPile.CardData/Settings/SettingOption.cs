namespace CardPile.CardData.Settings;

public class SettingOption : Setting, ICardDataSourceSettingOption
{
    public SettingOption(string name, bool value) : base(name, SettingType.Option)
    {
        Value = value;
    }

    public bool Value { get; set; }
}
