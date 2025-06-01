using CardPile.Application.Services;
using ReactiveUI;
using System.Collections.Generic;

namespace CardPile.Application.Models;

internal class DraftPackModel : ReactiveObject, IDraftPackService
{
    internal DraftPackModel(int packNumber, int pickNumber, List<ICardDataService> cards)
    {
        PackNumber = packNumber;
        PickNumber = pickNumber;
        Cards = cards;
    }

    public int PackNumber { get; init; }

    public int PickNumber { get; init; }

    public List<ICardDataService> Cards { get; init; }
}
