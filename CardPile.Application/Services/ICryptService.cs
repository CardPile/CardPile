using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CardPile.Application.Services;

internal interface ICryptService : IReactiveObject
{
    public ObservableCollection<ISkeletonService> Skeletons { get; }

    public void SetCardDataSource(ICardDataSource cardDataSource);

    public void UpdateSkeletons(List<int> cardIds);
}
