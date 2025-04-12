namespace CardPile.CardData;

public interface ICardDataSourceSettingDecimal : ICardDataSourceSetting
{
    public decimal Value { get; set; }

    public decimal MaxAllowedValue { get; }

    public decimal MinAllowedValue { get; }
}
