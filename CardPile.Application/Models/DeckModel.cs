using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CardPile.Application.Services;
using CardPile.Deck;

namespace CardPile.Application.Models;

public class DeckModel : IDeckService
{
    public void SetDeck(DraftDeck deck)
    {
        CardStacks.Clear();
        
        foreach (var stack in deck.CardStacks)
        {
            CardStacks.Add(stack.Select(card => new CardDataModel(card)).ToList<ICardDataService>());
        }
    }

    public void Clear()
    {
        CardStacks.Clear();
    }

    public ObservableCollection<List<ICardDataService>> CardStacks { get; init; } = [];
}