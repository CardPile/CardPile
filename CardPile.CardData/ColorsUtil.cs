namespace CardPile.CardData;

public static class ColorsUtil
{
    public static List<Color> ColorPairs()
    {
        return
        [
            Color.WU,
            Color.WB,
            Color.WR,
            Color.WG,
            Color.UB,
            Color.UR,
            Color.UG,
            Color.BR,
            Color.BG,
            Color.RG
        ];
    }
    
    public static string ToSymbols(Color color)
    {
        string result = "";
        if (color.HasFlag(Color.White))
        {
            result += "{W}";
        }
        if (color.HasFlag(Color.Blue))
        {
            result += "{U}";
        }
        if (color.HasFlag(Color.Black))
        {
            result += "{B}";
        }
        if (color.HasFlag(Color.Red))
        {
            result += "{R}";
        }
        if (color.HasFlag(Color.Green))
        {
            result += "{G}";
        }

        if (string.IsNullOrWhiteSpace(result))
        {
            result = "{C}";
        }
    
        return result;
    }

    public static Color CombineColors(List<Color> colors)
    {
        Color result = Color.None;
        foreach (var color in colors)
        {
            result |= color;
        }
        return result;
    }
}