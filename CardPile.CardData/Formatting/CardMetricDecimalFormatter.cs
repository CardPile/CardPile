namespace CardPile.CardData.Formatting;

public class CardMetricDecimalFormatter : ICardMetricFormatter<float>
{
    public string Format(float? value)
    {
        if (!value.HasValue)
        {
            return string.Empty;
        }

        return string.Format("{0:0.00}", value.Value);
    }
}
