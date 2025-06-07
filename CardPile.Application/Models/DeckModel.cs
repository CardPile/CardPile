using System;
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

    public void UpdateDeck(List<ICardData> cards, Func<ICardDataService, ICardDataService> annotator)
    {
        deck.UpdateDeck(cards);

        CardStacks.Clear();
        
        foreach (var stack in deck.CardStacks)
        {
            CardStacks.Add([.. stack.Select(card => annotator(new CardDataModel(card)))]);
        }
    }

    public void Clear()
    {
        CardStacks.Clear();
    }

    public ObservableCollection<List<ICardDataService>> CardStacks { get; init; } = [];

    private DraftDeck deck;
}