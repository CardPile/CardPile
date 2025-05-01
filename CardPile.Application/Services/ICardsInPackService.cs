using ReactiveUI;
using System.Collections.ObjectModel;

namespace CardPile.Application.Services;

internal interface ICardsInPackService : IReactiveObject
{
    public ObservableCollection<ICardDataService> CardsInPack { get; }

    public ObservableCollection<ICardDataService> CardsMissingFromPack { get; }

    public ObservableCollection<ICardDataService> CardsUpcomingAfterPack { get; }

    public ObservableCollection<ICardDataService> CardsSeen { get; }

    public IDeckService Deck { get; }
    
    public ICardDataService? PreviousPick { get; }

    public void ClearPersistentState();
}
