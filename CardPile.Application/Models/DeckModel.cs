using System.Collections.Generic;
using System.Collections.ObjectModel;
using CardPile.Application.Services;
using CardPile.Deck;

namespace CardPile.Application.Models;

public class DeckModel : IDeckService
{
    public void SetDeck(DraftDeck deck)
    {
        AllCards.Clear();
        
        foreach (var card in deck.Cards)
        {
            AllCards.Add(new CardDataModel(card));
        }
    }

    public void Clear()
    {
        AllCards.Clear();
    }

    public ObservableCollection<CardDataModel> AllCards {  get; } = [];
}