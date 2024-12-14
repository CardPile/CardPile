namespace CardPile.CardData.Formatting;

public class PercentFormatter : IFormatter<float>
{
    public PercentFormatter(bool showPositiveSign = false)
    {
        ShowPositiveSign = showPositiveSign;
    }

    public bool ShowPositiveSign { get; init; }

    public string Format(float? value)
    {
        if (!value.HasValue)
        {
            return string.Empty;
        }

        return string.Format("{0}{1:0.00}%", ShowPositiveSign && value.Value >= 0.0 ? "+" : "", 100.0f * value.Value);
    }
}
