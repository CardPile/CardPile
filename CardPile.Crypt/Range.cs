namespace CardPile.Crypt;

public class Range
{
    public Range()
    {
        From = 0;
        To = int.MaxValue;
    }

    internal int From { get; set; }

    internal int To { get; set; }

    public bool Empty { get => (To < From); }

    public bool Infinite { get => To == int.MaxValue; }

    public string TextValue
    {
        get => ToText();
    }

    public bool Contains(int value)
    {
        return From <= value && value <= To;
    }

    public string ToText()
    {
        if (From == 0 && To == int.MaxValue)
        {
            return "0+";
        }

        if (From == To)
        {
            return From.ToString();
        }

        if (From == 0)
        {
            return To.ToString() + "-";
        }

        if (To == int.MaxValue)
        {
            return From.ToString() + "+";
        }

        return From.ToString() + "-" + To.ToString();
    }

    internal static Range Intersect(Range lhs, Range rhs)
    {
        return new() { From = Math.Max(lhs.From, rhs.From), To = Math.Min(lhs.To, rhs.To) };
    }

    internal void Extend(Range other)
    {
        From = From != int.MaxValue && other.From != int.MaxValue ? From + other.From : int.MaxValue;
        To = To != int.MaxValue && other.To != int.MaxValue ? To + other.To : int.MaxValue;
    }

    internal static Range? TryParse(string str)
    {
        if (str.EndsWith('+'))
        {
            if (int.TryParse(str.AsSpan(0, str.Length - 1), out int from))
            {
                return new Range { From = from, To = int.MaxValue };
            }
        }
        else if (str.EndsWith('-'))
        {
            if (int.TryParse(str.AsSpan(0, str.Length - 1), out int to))
            {
                return new Range { From = 0, To = to };
            }
        }
        else if (str.Contains('-'))
        {
            var parts = str.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && int.TryParse(parts[0], out int rangeFrom) && int.TryParse(parts[1], out int rangeTo))
            {
                return new Range { From = rangeFrom, To = rangeTo };
            }
        }
        else
        {
            if (int.TryParse(str, out int value))
            {
                return new Range { From = value, To = value };
            }
        }

        return null;
    }


}
