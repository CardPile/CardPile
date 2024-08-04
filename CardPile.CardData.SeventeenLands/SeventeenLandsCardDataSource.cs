namespace CardPile.CardData.SeventeenLands;

public class SeventeenLandsCardDataSource : ICardDataSource
{
    internal SeventeenLandsCardDataSource()
    { }

    internal SeventeenLandsCardDataSource(List<SeventeenLandsCardData> cardData)
    {
        cardDataSet = cardData.ToDictionary(x => x.ArenaCardId, x => x);
    }

    public string Name => "17Lands";

    public ICardData? GetDataForCard(int cardNumber)
    {
        if(cardDataSet.TryGetValue(cardNumber, out SeventeenLandsCardData? cardData))
        {
            return cardData;
        }

        string? cardNameFromArena = CardInfo.Arena.GetCardNameFromId(cardNumber);
        if (cardNameFromArena != null)
        {
            var (expansion, collectorNumber) = CardInfo.Arena.GetCardExpansionAndCollectorNumberFromId(cardNumber);
            string? url = null;
            if (expansion != null && collectorNumber != null)
            {
                url = CardInfo.Scryfall.GetImageUrlFromExpansionAndCollectorNumber(expansion, collectorNumber);
            }

            var colors = CardInfo.Arena.GetCardColorsFromId(cardNumber) ?? [];
            return new SeventeenLandsCardData(cardNameFromArena, cardNumber, colors, url);
        }

        return null;
    }

    private readonly Dictionary<int, SeventeenLandsCardData> cardDataSet = [];
}
