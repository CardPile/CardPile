namespace CardPile.Crypt;

public class Range
{
    public Range()
    {
        From = 0;
        To = 0;
    }

    internal int From;

    internal int To;

    public string TextValue
    {
        get => ConvertToText();
    }

    internal bool IsEmpty()
    {
        return (To < From);
    }

    internal void Extend(Range other)
    {
        From = From != int.MaxValue && other.From != int.MaxValue ? From + other.From : int.MaxValue;
        To = To != int.MaxValue && other.To != int.MaxValue ? To + other.To : int.MaxValue;
    }

    internal static Range Intersect(Range lhs, Range rhs)
    {
        return new() { From = Math.Max(lhs.From, rhs.From), To = Math.Min(lhs.To, rhs.To) };
    }

    private string ConvertToText()
    {
        if(From == To)
        {
            return From.ToString();
        }

        if(From == 0)
        {
            return To.ToString() + "-";
        }

        if(To == int.MaxValue)
        {
            return From.ToString() + "+";
        }

        return From.ToString() + "-" + To.ToString();
    }
}
