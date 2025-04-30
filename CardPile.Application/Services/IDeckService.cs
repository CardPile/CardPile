using System.Collections.Generic;
using System.Collections.ObjectModel;
using CardPile.Deck;
using ReactiveUI;

namespace CardPile.Application.Services;

public interface IDeckService : IReactiveObject
{
    public ObservableCollection<List<ICardDataService>> CardStacks { get; }
    
    public void SetDeck(DraftDeck deck);
    
    public void Clear();
}