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
    
    public static string ToEmoji(Color color)
    {
        string result = "";
        if (color.HasFlag(Color.White))
        {
            result += "\u26aa";
        }
        if (color.HasFlag(Color.Blue))
        {
            result += "\ud83d\udd35";
        }
        if (color.HasFlag(Color.Black))
        {
            result += "\u26ab";
        }
        if (color.HasFlag(Color.Red))
        {
            result += "\ud83d\udd34";
        }
        if (color.HasFlag(Color.Green))
        {
            result += "\ud83d\udfe2";
        }

        if (string.IsNullOrWhiteSpace(result))
        {
            result = "\u26aa";
        }
    
        return result;
    }
}