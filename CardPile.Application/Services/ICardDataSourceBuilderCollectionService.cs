using ReactiveUI;
using System.Collections.Generic;

namespace CardPile.Application.Services;

internal interface ICardDataSourceBuilderCollectionService : IReactiveObject
{
    internal List<ICardDataSourceBuilderService> AvailableCardDataSourceBuilders { get; }

    internal ICardDataSourceBuilderService CurrentCardDataSourceBuilder { get; set; }
}
