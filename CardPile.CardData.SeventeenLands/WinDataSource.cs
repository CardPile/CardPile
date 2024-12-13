namespace CardPile.CardData.SeventeenLands;

internal class WinDataSource
{
    internal WinDataSource(List<RawWinData> winData)
    {
        archetypeWinData = winData.Where(x => !x.IsSummary.GetValueOrDefault(true)).ToDictionary(x => CombineColors(x.Colors), x => x);
    }

    internal float GetWinPercentage(Color color)
    {
        if (!archetypeWinData.TryGetValue(color, out var winData))
        {
            return float.NaN;
        }

        if(!winData.Wins.HasValue || !winData.Games.HasValue)
        {
            return float.NaN;
        }

        return (float)Math.Round((double)winData.Wins / (double)winData.Games, 3);
    }

    private Color CombineColors(List<Color> colors)
    {
        Color result = Color.None;
        foreach (var color in colors)
        {
            result |= color;
        }

        return result;
    }

    private Dictionary<Color, RawWinData> archetypeWinData = [];
}
