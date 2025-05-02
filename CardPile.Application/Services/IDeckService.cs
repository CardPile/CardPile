using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CardPile.CardData;
using ReactiveUI;

namespace CardPile.Application.Services;

public interface IDeckService : IReactiveObject
{
    public ObservableCollection<List<ICardDataService>> CardStacks { get; }
    
    public void UpdateDeck(List<ICardData> cards, Func<ICardDataService, ICardDataService> annotator);
    
    public void Clear();
}