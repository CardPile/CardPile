namespace CardPile.CardData;

public interface ICardDataSourceSetting
{
    public string Name { get; }

    public CardDataSourceSettingType Type { get; }
}
