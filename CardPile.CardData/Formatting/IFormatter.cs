namespace CardPile.CardData.Formatting;

public interface IFormatter<T> where T : struct
{
    public string Format(T? value);
}
