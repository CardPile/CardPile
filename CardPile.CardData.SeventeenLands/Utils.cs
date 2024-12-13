namespace CardPile.CardData.SeventeenLands;

internal class Utils
{
    internal static List<Color> ParseColors(string? colors)
    {
        if (colors == null)
        {
            return [];
        }

        var result = new List<Color>();
        foreach (var color in colors)
        {
            if (color == 'W')
            {
                result.Add(Color.White);
            }
            else if (color == 'U')
            {
                result.Add(Color.Blue);
            }
            else if (color == 'B')
            {
                result.Add(Color.Black);
            }
            else if (color == 'R')
            {
                result.Add(Color.Red);
            }
            else if (color == 'G')
            {
                result.Add(Color.Green);
            }
        }

        return result;
    }
}
