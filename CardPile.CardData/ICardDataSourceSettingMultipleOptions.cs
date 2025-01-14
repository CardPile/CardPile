namespace CardPile.CardData;

public interface ICardDataSourceSettingMultipleOptions : ICardDataSourceSetting
{
    public List<ICardDataSourceSettingOption> Options { get; }
}
