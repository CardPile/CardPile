namespace CardPile.CardData.SeventeenLands;

internal class WinDataSource
{
    internal WinDataSource(List<RawWinData> winData)
    {
        archetypeWinData = winData.Where(x => !x.IsSummary.GetValueOrDefault(true)).ToDictionary(x => x.Colors, x => x);
    }

    internal float? GetWinPercentage(Color color)
    {
        if (!archetypeWinData.TryGetValue(color, out var winData))
        {
            return null;
        }

        if (!winData.Wins.HasValue || !winData.Games.HasValue)
        {
            return null;
        }

        return (float)Math.Round((double)winData.Wins / (double)winData.Games, 3);
    }

    private Dictionary<Color, RawWinData> archetypeWinData = [];
}
