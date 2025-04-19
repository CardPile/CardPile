namespace CardPile.CardData.Settings;

public class SettingDecimal : Setting, ICardDataSourceSettingDecimal
{
    public SettingDecimal(string name, decimal value, decimal minAllowedValue = decimal.MinValue, decimal maxAllowedValue = decimal.MaxValue, decimal increment = 1.0m) : base(name, SettingType.Decimal)
    {
        MinAllowedValue = minAllowedValue;
        MaxAllowedValue = maxAllowedValue;
        Increment = increment;
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

    public decimal Increment { get; init; }

    private decimal value;
}
