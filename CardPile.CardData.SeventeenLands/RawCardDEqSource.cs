namespace CardPile.CardData.SeventeenLands;

internal class RawCardDEqSource
{
    internal RawCardDEqSource(List<RawCardDEq> deq)
    {
        deqSet = deq.ToDictionary(x => x.Name, x => x);
    }
    
    public RawCardDEq? GetDEqCard(string name)
    {
        return deqSet.TryGetValue(name, out RawCardDEq? deq) ? deq : null;
    }
    
    private readonly Dictionary<string, RawCardDEq> deqSet;
}