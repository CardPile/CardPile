using NLog;

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
        if (From == To)
        {
            return From.ToString();
        }

        if (To == int.MaxValue)
        {
            return From.ToString() + "+";
        }

        if (From == 1)
        {
            return To.ToString() + "-";
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
                if(from < 1)
                {
                    logger.Error("Half open range should start with a number greater of equal to 1 (currently {0}+)", from);
                    return null;
                }

                return new Range { From = from, To = int.MaxValue };
            }
        }
        else if (str.EndsWith('-'))
        {
            if (int.TryParse(str.AsSpan(0, str.Length - 1), out int to))
            {
                if (to < 1)
                {
                    logger.Error("Half open range should end with a number greater of equal to 1 (currently {0}-)", to);
                    return null;
                }

                return new Range { From = 1, To = to };
            }
        }
        else if (str.Contains('-'))
        {
            var parts = str.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && int.TryParse(parts[0], out int from) && int.TryParse(parts[1], out int to))
            {
                if(from < 1)
                {
                    logger.Warn("Range should start with a number greater or equal to 1. Currently {0}-{1}.", from, to);
                    return null;
                }
                if(to < 1)
                {
                    logger.Warn("Range should end with a number greater of equal to 1. Currently {0}-{1}.", from, to);
                    return null;
                }
                if(from > to)
                {
                    logger.Warn("Range start should be less or equal to it's end. Currently {0}-{1}.", from, to);
                    return null;
                }

                return new Range { From = from, To = to };
            }
        }
        else
        {
            if (int.TryParse(str, out int value))
            {
                if(value < 1)
                {
                    logger.Warn("Single value range must consist of a value greater or equal to 1. Currently {0}.", value);
                    return null;
                }
                return new Range { From = value, To = value };
            }
        }

        return null;
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
