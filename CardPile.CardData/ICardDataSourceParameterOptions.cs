namespace CardPile.CardData;

public interface ICardDataSourceParameterOptions : ICardDataSourceParameter
{
    public List<string> Options { get; }

    public string Value { get; set; }
}
