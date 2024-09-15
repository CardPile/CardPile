namespace CardPile.CardData.SeventeenLands;

public class SeventeenLandsCardDataSource : ICardDataSource
{
    internal SeventeenLandsCardDataSource()
    { }

    internal SeventeenLandsCardDataSource(SeventeenLandsRawCardDataSource cardData,
                                          SeventeenLandsRawCardDataSource wuCardData,
                                          SeventeenLandsRawCardDataSource wbCardData,
                                          SeventeenLandsRawCardDataSource wrCardData,
                                          SeventeenLandsRawCardDataSource wgCardData,
                                          SeventeenLandsRawCardDataSource ubCardData,
                                          SeventeenLandsRawCardDataSource urCardData,
                                          SeventeenLandsRawCardDataSource ugCardData,
                                          SeventeenLandsRawCardDataSource brCardData,
                                          SeventeenLandsRawCardDataSource bgCardData,
                                          SeventeenLandsRawCardDataSource rgCardData)
    {
        cardDataSet = cardData.CardDataSet.ToDictionary(x => x.Value.ArenaCardId, x => new SeventeenLandsCardData(x.Value,
                                                                                                                  (wuCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value,
                                                                                                                  (wbCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value,
                                                                                                                  (wrCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value,
                                                                                                                  (wgCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value,
                                                                                                                  (ubCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value,
                                                                                                                  (urCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value,
                                                                                                                  (ugCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value,
                                                                                                                  (brCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value,
                                                                                                                  (bgCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value,
                                                                                                                  (rgCardData.GetDataForCard(x.Value.ArenaCardId) as SeventeenLandsRawCardData)?.WinRateInHandMetric.Value));
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
