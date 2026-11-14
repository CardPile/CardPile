using NLog;

namespace CardPile.CardData.SeventeenLands;

internal class RawCardLluSource
{
    internal RawCardLluSource(List<RawCardLluRating> lluRatings)
    {
        lluRatingSet = lluRatings.ToDictionary(x => x.Name, x => x);
    }
    
    public RawCardLluRating? GetLluCardRating(string name)
    {
        var result = lluRatingSet.TryGetValue(name, out RawCardLluRating? lluRating) ? lluRating : null;
        
        if (result == null)
        {
            logger.Warn("No LLU rating found for {name}", name);
        }

        return result;
    }
    
    private readonly Dictionary<string, RawCardLluRating> lluRatingSet;

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}