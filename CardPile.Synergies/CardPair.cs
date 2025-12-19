namespace CardPile.Synergies;

internal class CardPair : IEquatable<CardPair>
{
    public string CardA {  get; init; }

    public string CardB { get; init; }

    public CardPair(string cardA, string cardB)
    {
        CardA = cardA;
        CardB = cardB;
    }

    public bool Equals(CardPair? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return (CardA.Equals(other.CardA) && CardB.Equals(other.CardB)) || (CardA.Equals(other.CardB) && CardB.Equals(other.CardA));
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return Equals(obj as CardPair);
    }

    public override int GetHashCode()
    {
        var hashX = CardA.GetHashCode();
        var hashY = CardB.GetHashCode();
        return HashCode.Combine(Math.Min(hashX, hashY), Math.Max(hashX, hashY));
    }

    public static bool operator ==(CardPair left, CardPair right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CardPair left, CardPair right)
    {
        return !Equals(left, right);
    }
}
