namespace CardPile.CardInfo;

public class Scryfall
{
    public static string? GetImageUrlFromExpansionAndCollectorNumber(string expansion, string collectorNumber)
    {
        return $"https://api.scryfall.com/cards/{expansion.ToLower()}/{collectorNumber}?format=image";
    }
}
