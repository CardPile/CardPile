using CardPile.CardData;

namespace CardPile.Deck;

public class DraftDeck
{
    public DraftDeck(List<ICardData> cards)
    {
        var colorCount = CalculateColorCounts(cards);
        var mainColors = CalculateMainColors(colorCount);
        var (mainCards, sideboardCards) = SplitCards(cards, mainColors);

        if (mainCards.Count > 0)
        {
            // Split main cards by mana value, then sort each stack by color
            // Sort the sideboard cards by mana value for the last stack
            CardStacks = SplitCardsByManaValue(mainCards);
            CardStacks.Add(sideboardCards);
        }
        else
        {
            // No main colors - split sideboard cards by mana value, then sort each stack by color
            CardStacks = SplitCardsByManaValue(sideboardCards);
        }
        
        foreach (var cardStack in CardStacks)
        {
            SortStackByColor(cardStack);
        }        
        
        Cards = cards;
    }
    
    public List<ICardData> Cards { get; init; }

    public List<List<ICardData>> CardStacks { get; init; }
    
    private Dictionary<Color, int> CalculateColorCounts(List<ICardData> cards)
    {
        Dictionary<Color, int> colorCounts = new() {
            { Color.White, 0},
            { Color.Blue, 0},
            { Color.Black, 0},
            { Color.Red, 0},
            { Color.Green, 0},
        };
        
        foreach (var color in cards.SelectMany(card => card.Colors))
        {
            colorCounts[color] += 1;
        }

        return colorCounts;
    }

    private List<Color> CalculateMainColors(Dictionary<Color, int> colorCounts)
    {
        List<Color> mainColors = [];
        foreach (var (color, count) in colorCounts)
        {
            if (count > 3)
            {
                mainColors.Add(color);
            }
        }
        
        return mainColors;
    }

    private (List<ICardData>, List<ICardData>) SplitCards(List<ICardData> cards, List<Color> mainColors)
    {
        List<ICardData> mainboard = [];
        List<ICardData> sideboard = [];
        foreach (var card in cards)
        {
            if (card.Colors.All(c => mainColors.Contains(c)))
            {
                mainboard.Add(card);
            }
            else
            {
                sideboard.Add(card);
            }
        }

        return (mainboard, sideboard);
    }

    private List<List<ICardData>> SplitCardsByManaValue(List<ICardData> cards)
    {
        Dictionary<int, List<ICardData>> cardsByManaValue = [];
        foreach (var card in cards)
        {
            var manaValue = card.ManaValue ?? int.MaxValue;
            cardsByManaValue.TryAdd(manaValue, []);
            cardsByManaValue[manaValue].Add(card);
        }

        var result = cardsByManaValue.ToList();
        result.Sort((x, y) => x.Key - y.Key);
        return result.Select(x => x.Value).ToList();
    }

    private void SortStackByColor(List<ICardData> cardStack)
    {
        cardStack.Sort((x, y) => ColorsUtil.CombineColors(x.Colors) - ColorsUtil.CombineColors(y.Colors));
    }
}