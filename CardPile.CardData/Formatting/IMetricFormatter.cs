namespace CardPile.CardData.Formatting;

public interface IMetricFormatter<T> where T : struct
{
    public string Format(T? value);
}
