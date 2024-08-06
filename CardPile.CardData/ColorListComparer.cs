namespace CardPile.CardData;

public class ColorListComparer : IComparer<List<Color>?>
{
    public int Compare(List<Color>? x, List<Color>? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }

        if (x == null)
        {
            return -1;
        }

        if (y == null)
        {
            return 1;
        }

        if (x.Count == 0)
        {
            return y.Count == 0 ? 0 : 1;
        }

        if (x.Count > 1)
        {
            if(y.Count == 0)
            {
                return -1;
            }
            if (y.Count == 1)
            {
                return 1;
            }
            return 0;
        }

        if (y.Count == 0 || y.Count > 1)
        {
            return -1;
        }

        return colorComparer.Compare(x.First(), y.First());
    }

    private ColorComparer colorComparer = new ColorComparer();
};