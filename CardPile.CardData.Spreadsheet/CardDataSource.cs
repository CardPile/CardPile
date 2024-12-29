using CardPile.Draft;

namespace CardPile.CardData.Spreadsheet;

public class CardDataSource : ICardDataSource
{
    internal CardDataSource()
    { }

    internal CardDataSource(IEnumerable<SpreadsheetEntry> entries)
    {
        cardGrades = entries.ToDictionary(x => x.Name, x => x.Grade);
    }

    public string Name => "Spreadsheet";

    public ICardData? GetDataForCard(int cardNumber, DraftState draftState)
    {
        string? cardNameFromArena = CardInfo.Arena.GetCardNameFromId(cardNumber);
        if (cardNameFromArena == null)
        {
            return null;
        }

        var (expansion, collectorNumber) = CardInfo.Arena.GetCardExpansionAndCollectorNumberFromId(cardNumber);
        string? url = null;
        if (expansion != null && collectorNumber != null)
        {
            url = CardInfo.Scryfall.GetImageUrlFromExpansionAndCollectorNumber(expansion, collectorNumber);
        }

        string? grade = null;
        cardGrades.TryGetValue(cardNameFromArena, out grade);

        var type = CardInfo.Arena.GetCardTypeFromId(cardNumber);
        var manaValue = CardInfo.Arena.GetCardManaValueFromId(cardNumber);
        var colors = CardInfo.Arena.GetCardColorsFromId(cardNumber);
        return new CardData(cardNameFromArena, cardNumber, type, manaValue, colors, url, grade);
    }

    public List<ICardDataSourceStatistic> Statistics { get => []; }

    private readonly Dictionary<string, string> cardGrades = [];
}