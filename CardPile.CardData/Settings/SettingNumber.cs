namespace CardPile.CardData.Settings;

public class SettingNumber : Setting, ICardDataSourceSettingNumber
{
    public SettingNumber(string name, int value, int minAllowedValue = int.MinValue, int maxAllowedValue = int.MaxValue ) : base(name, SettingType.Number)
    {
        MinAllowedValue = minAllowedValue;
        MaxAllowedValue = maxAllowedValue;
        Value = value;
    }

    public int Value 
    {
        get => value;
        set
        {
            this.value = Math.Clamp(value, MinAllowedValue, MaxAllowedValue);
        }
    }

    public int MinAllowedValue { get; init; }

    public int MaxAllowedValue { get; init; }

    private int value;
}
