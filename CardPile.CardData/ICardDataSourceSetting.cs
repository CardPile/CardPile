using CardPile.CardData.Settings;

namespace CardPile.CardData;

public interface ICardDataSourceSetting
{
    public string Name { get; }

    public SettingType Type { get; }
}
