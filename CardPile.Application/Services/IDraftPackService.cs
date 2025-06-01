using ReactiveUI;
using System.Collections.Generic;

namespace CardPile.Application.Services;

internal interface IDraftPackService : IReactiveObject
{
    public int PackNumber { get; }

    public int PickNumber { get; }

    public List<ICardDataService> Cards { get; }
}
