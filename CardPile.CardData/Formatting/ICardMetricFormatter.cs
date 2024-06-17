namespace CardPile.CardData.Formatting;

public interface ICardMetricFormatter<T> where T : struct
{
    public string Format(T? value);
}
