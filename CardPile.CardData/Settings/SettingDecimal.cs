namespace CardPile.CardData.Settings;

public class SettingDecimal : Setting, ICardDataSourceSettingDecimal
{
    public SettingDecimal(string name, decimal value, decimal minAllowedValue = decimal.MinValue, decimal maxAllowedValue = decimal.MaxValue) : base(name, SettingType.Decimal)
    {
        MinAllowedValue = minAllowedValue;
        MaxAllowedValue = maxAllowedValue;
        Value = value;
    }

    public decimal Value
    {
        get => value;
        set
        {
            this.value = Math.Clamp(value, MinAllowedValue, MaxAllowedValue);
        }
    }

    public decimal MinAllowedValue { get; init; }

    public decimal MaxAllowedValue { get; init; }

    private decimal value;
}
