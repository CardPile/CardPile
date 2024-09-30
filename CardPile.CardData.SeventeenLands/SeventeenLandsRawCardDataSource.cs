using System.Collections;

namespace CardPile.CardData.SeventeenLands;

internal class SeventeenLandsRawCardDataSource : IEnumerable<KeyValuePair<int, SeventeenLandsRawCardData>>
{
    internal SeventeenLandsRawCardDataSource(List<SeventeenLandsRawCardData> cardData)
    {
        cardDataSet = cardData.ToDictionary(x => x.ArenaCardId, x => x);
    }

    public SeventeenLandsRawCardData? GetDataForCard(int cardNumber)
    {
        return cardDataSet.TryGetValue(cardNumber, out SeventeenLandsRawCardData? cardData) ? cardData : null;
    }

    public IEnumerator<KeyValuePair<int, SeventeenLandsRawCardData>> GetEnumerator()
    {
        return cardDataSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return cardDataSet.GetEnumerator();
    }

    private readonly Dictionary<int, SeventeenLandsRawCardData> cardDataSet = [];
}
