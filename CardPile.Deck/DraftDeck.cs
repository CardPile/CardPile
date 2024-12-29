using Avalonia.Media;
using CardPile.CardData;
using Color = CardPile.CardData.Color;
using Type = CardPile.CardData.Type;

namespace CardPile.Deck;

public class DraftDeck
{
    public DraftDeck(List<ICardData> cards)
    {
        const int MAIN_COLOR_COUNT = 2;
        
        var cardGroups = GroupCards(cards, MAIN_COLOR_COUNT);
        var mainColorAndCards = cardGroups.MaxBy(x => x.Value.Count);
        var sideboardCards = SplitSideboard(cards, mainColorAndCards.Key);

        if (mainColorAndCards.Value.Count > 0)
        {
            // Split main cards by mana value, then sort each stack by color
            // Sort the sideboard cards by mana value for the last stack
            CardStacks = SplitCardsByManaValue(mainColorAndCards.Value);
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
    }
    
    public List<List<ICardData>> CardStacks { get; init; }
    
    private Dictionary<Color, List<ICardData>> GroupCards(List<ICardData> cards, int numberOfColorsToConsider)
    {
        var colorCombinationCounts = ColorsUtil.Colors(numberOfColorsToConsider)
                                                                    .ToDictionary(x => x,
                                                                                  _ => new List<ICardData>());
        foreach (var colorCombination in colorCombinationCounts.Keys)
        {
            foreach (var card in cards)
            {
                if ((colorCombination & card.Colors) == card.Colors)
                {
                    colorCombinationCounts[colorCombination].Add(card);
                }
            }
        }
        return colorCombinationCounts;
    }
    
    private List<ICardData> SplitSideboard(List<ICardData> cards, Color mainColors)
    {
        List<ICardData> sideboard = [];
        foreach (var card in cards)
        {
            if ((card.Colors & mainColors) != card.Colors)
            {
                sideboard.Add(card);
            }
        }

        return sideboard;
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
        
        var cardsSortedByManaValue = cardsByManaValue.ToList();
        cardsSortedByManaValue.Sort((x, y) => x.Key - y.Key);
        var result = cardsSortedByManaValue.Select(x => x.Value).ToList();

        if (cardsSortedByManaValue.Count <= 0 || cardsSortedByManaValue.First().Key != 0)
        {
            return result;
        }
        
        var lands = result[0].Where(card => (card.Type & Type.Land) == Type.Land).ToList();
        var zeroManaWithoutLands = result[0].Except(lands).ToList();
        if (zeroManaWithoutLands.Count > 0)
        {
            result[0] = zeroManaWithoutLands;
        }
        else
        {
            result.RemoveAt(0);
        }
        if (lands.Count > 0)
        {
            result.Add(lands);                
        }

        return  result;
    }

    private void SortStackByColor(List<ICardData> cardStack)
    {
        cardStack.Sort((x, y) => x.Colors - y.Colors);
    }
}