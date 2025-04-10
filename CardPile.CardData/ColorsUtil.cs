using System.Collections.Generic;
using System.Linq;

namespace CardPile.CardData;

public static class ColorsUtil
{
    public static List<Color> ColorSingles()
    {
        return
        [
            Color.White,
            Color.Blue,
            Color.Black,
            Color.Red,
            Color.Green,
        ];
    }

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

    public static List<Color> ColorTriples()
    {
        return
        [
            Color.WUB,
            Color.WUR,
            Color.WUG,
            Color.WBR,
            Color.WBG,
            Color.WRG,
            Color.UBR,
            Color.UBG,
            Color.URG,
            Color.BRG
        ];
    }

    public static List<Color> ColorQuadruples()
    {
        return
        [
            Color.WUBR,
            Color.WUBG,
            Color.WURG,
            Color.WBRG,
            Color.UBRG,
        ];
    }

    public static List<Color> ColorQuintuples()
    {
        return
        [
            Color.WUBRG,
        ];
    }

    public static List<Color> Colors(int count)
    {
        return count switch
        {
            0 => [Color.None],
            1 => ColorSingles(),
            2 => ColorPairs(),
            3 => ColorTriples(),
            4 => ColorQuadruples(),
            5 => ColorQuintuples(),
            _ => []
        };
    }

    public static List<Color> Colors(params int[] counts)
    {
        List<Color> result = [];
        foreach (int count in counts)
        {
            result.AddRange(Colors(count));
        }
        return result;
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

    public static string ToWUBRG(Color color)
    {
        string result = "";
        if (color.HasFlag(Color.White))
        {
            result += "W";
        }
        if (color.HasFlag(Color.Blue))
        {
            result += "U";
        }
        if (color.HasFlag(Color.Black))
        {
            result += "B";
        }
        if (color.HasFlag(Color.Red))
        {
            result += "R";
        }
        if (color.HasFlag(Color.Green))
        {
            result += "G";
        }

        if (string.IsNullOrWhiteSpace(result))
        {
            result += "C";
        }

        return result;
    }

    public static int Count(this Color color)
    {
        var result = 0;
        if ((color & Color.White) != Color.None)
        {
            ++result;
        }
        if ((color & Color.Blue) != Color.None)
        {
            ++result;
        }
        if ((color & Color.Black) != Color.None)
        {
            ++result;
        }
        if ((color & Color.Red) != Color.None)
        {
            ++result;
        }
        if ((color & Color.Green) != Color.None)
        {
            ++result;
        }

        return result;
    }
}