namespace CardPile.CardData;

public interface ICardDataSourceSettingNumber : ICardDataSourceSetting
{
    public int Value { get; set; }

    public int MaxAllowedValue { get; }

    public int MinAllowedValue { get; }
}
