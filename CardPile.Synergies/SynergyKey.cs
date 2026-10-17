namespace CardPile.Synergies;

public class SynergyKey
{
    public SynergyKey(string firstCard, string secondCard)
    {
        if (string.Compare(firstCard, secondCard, StringComparison.OrdinalIgnoreCase) < 0)
        {
            FirstCard = firstCard;
            SecondCard = secondCard;
        }
        else
        {
            FirstCard = secondCard;
            SecondCard = firstCard;
        }
    }

    public string FirstCard { get; init; }

    public string SecondCard { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is SynergyKey other)
        {
            return string.Equals(FirstCard, other.FirstCard, StringComparison.OrdinalIgnoreCase)
                && string.Equals(SecondCard, other.SecondCard, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (FirstCard, SecondCard).GetHashCode();
    }

    public static bool operator ==(SynergyKey? left, SynergyKey? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(SynergyKey? left, SynergyKey? right)
    {
        return !(left == right);
    }
}
