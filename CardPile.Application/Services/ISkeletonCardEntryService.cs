using CardPile.Crypt;
using CardPile.CardData.Importance;

namespace CardPile.Application.Services;

internal interface ISkeletonCardEntryService : ICardDataService
{
    public Range? Range { get; }

    public ImportanceLevel Importance { get; }

    public int Count { get; }

    public bool IsSatisfied { get; }
}
