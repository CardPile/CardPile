namespace CardPile.CardData.Settings;

public class Setting : ICardDataSourceSetting
{
    protected Setting(string name, SettingType type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; init; }

    public SettingType Type { get; init; }
}
