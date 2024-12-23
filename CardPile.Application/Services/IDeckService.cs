using System.Collections.ObjectModel;
using CardPile.Application.Models;
using CardPile.Deck;

namespace CardPile.Application.Services;

public interface IDeckService
{
    internal ObservableCollection<CardDataModel> AllCards { get; }

    public void SetDeck(DraftDeck deck);
    
    public void Clear();
}