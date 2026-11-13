namespace CardPile.CardData.SeventeenLands;

internal class RawCardLluSource
{
    internal RawCardLluSource(List<RawCardLluRating> lluRatings)
    {
        lluRatingSet = lluRatings.ToDictionary(x => x.Name, x => x);
    }
    
    public RawCardLluRating? GetLluCardRating(string name)
    {
        return lluRatingSet.TryGetValue(name, out RawCardLluRating? lluRating) ? lluRating : null;
    }
    
    private readonly Dictionary<string, RawCardLluRating> lluRatingSet;
}