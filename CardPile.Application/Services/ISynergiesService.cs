using CardPile.CardData;
using ReactiveUI;

namespace CardPile.Application.Services;

internal interface ISynergiesService : IReactiveObject
{
    public void SetCardDataSource(ICardDataSource cardDataSource);
    
    public void AnnotateCard(ICardDataService card);
}
