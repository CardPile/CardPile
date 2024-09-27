namespace CardPile.CardData;

public class CardDataSourceSettingPath : CardDataSourceSetting, ICardDataSourceSettingPath
{
    public CardDataSourceSettingPath(string name, string path) : base(name, CardDataSourceSettingType.Path)
    {
        Value = path;
    }

    public string Value { get; set; }
}
