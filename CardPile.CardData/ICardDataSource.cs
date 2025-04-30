using CardPile.Draft;

namespace CardPile.CardData;

public interface ICardDataSource
{
    public string Name { get; }

    public ICardData? GetDataForCard(int cardNumber, DraftState? state = null);

    public List<ICardDataSourceStatistic> Statistics { get; }
}
