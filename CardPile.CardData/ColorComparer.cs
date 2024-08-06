namespace CardPile.CardData;

public class ColorComparer : IComparer<Color>
{
    public int Compare(Color x, Color y)
    {
        var xInt = (int)x;
        var yInt = (int)y;
        return xInt.CompareTo(yInt);
    }
}
