namespace CardPile.CardData.SeventeenLands;

internal class RawDEqSource
{
    internal RawDEqSource(List<RawDEq> deq)
    {
        deqSet = deq.ToDictionary(x => x.Name, x => x);
    }
    
    public RawDEq? GetDEqCard(string name)
    {
        return deqSet.TryGetValue(name, out RawDEq? deq) ? deq : null;
    }
    
    private readonly Dictionary<string, RawDEq> deqSet = [];
}