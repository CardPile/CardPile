namespace CardPile.CardData;

public class CardDataSourceSetting : ICardDataSourceSetting
{
    protected CardDataSourceSetting(string name, CardDataSourceSettingType type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; init; }

    public CardDataSourceSettingType Type { get; init; }
}
