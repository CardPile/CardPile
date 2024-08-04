namespace CardPile.CardData.Spreadsheet;

public class SpreadsheetCardDataSource : ICardDataSource
{
    internal SpreadsheetCardDataSource()
    { }

    internal SpreadsheetCardDataSource(IEnumerable<SpreadsheetEntry> entries)
    {
        cardGrades = entries.ToDictionary(x => x.Name, x => x.Grade);
    }

    public string Name => "Spreadsheet";

    public ICardData? GetDataForCard(int cardNumber)
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

        var colors = CardInfo.Arena.GetCardColorsFromId(cardNumber) ?? [];
        return new SpreadsheetCardData(cardNameFromArena, cardNumber, colors, grade);
    }

    private readonly Dictionary<string, string> cardGrades = [];
}