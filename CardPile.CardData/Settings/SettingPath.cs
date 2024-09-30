namespace CardPile.CardData.Settings;

public class SettingPath : Setting, ICardDataSourceSettingPath
{
    public SettingPath(string name, string path) : base(name, SettingType.Path)
    {
        Value = path;
    }

    public string Value { get; set; }
}
