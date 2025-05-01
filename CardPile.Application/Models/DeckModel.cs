using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.Deck;
using ReactiveUI;

namespace CardPile.Application.Models;

public class DeckModel : ReactiveObject, IDeckService
{
    public DeckModel()
    {
        deck = new DraftDeck();
    }

    public void UpdateDeck(List<ICardData> cards)
    {
        deck.UpdateDeck(cards);

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

    private DraftDeck deck;
}