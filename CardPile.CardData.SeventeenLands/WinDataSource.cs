namespace CardPile.CardData.SeventeenLands;

internal class WinDataSource
{
    internal WinDataSource(List<RawWinData> winData, float percentageCutoff)
    {
        var totalGameCount = winData.First(x => x.IsSummary.GetValueOrDefault(true) && NameToColorCount(x.ColorName) == int.MaxValue).Games;
        archetypeWinData = winData.Where(x => !x.IsSummary.GetValueOrDefault(true) && 100.0f * x.Games > percentageCutoff * totalGameCount).ToDictionary(x => x.Colors, x => x);
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

        return (float)Math.Round(winData.Wins.Value / winData.Games.Value, 3);
    }

    private int? NameToColorCount(string? name)
    {
        return name switch
        {
            "Mono-color" => 1,
            "Two-color" => 2,
            "Three-color" => 3,
            "Four-color" => 4,
            "Five-color" => 5,
            "All Decks" => int.MaxValue,
            _ => null
        };
    }

    private Dictionary<Color, RawWinData> archetypeWinData = [];
}
