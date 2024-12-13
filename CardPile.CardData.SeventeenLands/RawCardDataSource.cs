using System.Collections;

namespace CardPile.CardData.SeventeenLands;

internal class RawCardDataSource : IEnumerable<KeyValuePair<int, RawCardData>>
{
    internal RawCardDataSource(List<RawCardData> cardData)
    {
        cardDataSet = cardData.ToDictionary(x => x.ArenaCardId, x => x);
    }

    public RawCardData? GetDataForCard(int cardNumber)
    {
        return cardDataSet.TryGetValue(cardNumber, out RawCardData? cardData) ? cardData : null;
    }

    public IEnumerator<KeyValuePair<int, RawCardData>> GetEnumerator()
    {
        return cardDataSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return cardDataSet.GetEnumerator();
    }

    private readonly Dictionary<int, RawCardData> cardDataSet = [];
}
