namespace CardPile.CardData.SeventeenLands;

internal class SeventeenLandsRawCardDataSource : ICardDataSource
{
    internal SeventeenLandsRawCardDataSource()
    { }

    internal SeventeenLandsRawCardDataSource(List<SeventeenLandsRawCardData> cardData)
    {
        CardDataSet = cardData.ToDictionary(x => x.ArenaCardId, x => x);
    }

    public string Name => "17Lands";

    public ICardData? GetDataForCard(int cardNumber)
    {
        if(CardDataSet.TryGetValue(cardNumber, out SeventeenLandsRawCardData? cardData))
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
            return new SeventeenLandsRawCardData(cardNameFromArena, cardNumber, colors, url);
        }

        return null;
    }

    internal readonly Dictionary<int, SeventeenLandsRawCardData> CardDataSet = [];
}
