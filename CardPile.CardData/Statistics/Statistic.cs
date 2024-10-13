using CardPile.CardData.Formatting;

namespace CardPile.CardData.Metrics;

public class Statistic<T> : ICardDataSourceStatistic where T : struct
{
    public Statistic(string name, T? value, IFormatter<T>? formatter = null)
    {
        Name = name;
        Value = value;
        Formatter = formatter;
    }

    public string Name { get; init; }

    public string TextValue
    {
        get
        {
            if (Formatter != null)
            {
                return Formatter.Format(Value);
            }
            else
            {
                return Value?.ToString() ?? string.Empty;
            }
        }
    }

    public T? Value { get; init; }

    public IFormatter<T>? Formatter { get; init; }
}
