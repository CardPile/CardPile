namespace CardPile.CardData.SeventeenLands;

internal class Utils
{
    internal static Color ParseColors(string? colors)
    {
        if (colors == null)
        {
            return Color.None;
        }

        var result = Color.None;
        foreach (var color in colors)
        {
            if (color == 'W')
            {
                result |= Color.White;
            }
            else if (color == 'U')
            {
                result |= Color.Blue;
            }
            else if (color == 'B')
            {
                result |= Color.Black;
            }
            else if (color == 'R')
            {
                result |= Color.Red;
            }
            else if (color == 'G')
            {
                result |= Color.Green;
            }
        }

        return result;
    }
}
