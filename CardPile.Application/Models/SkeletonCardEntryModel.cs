using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.CardData.Importance;
using CardPile.Crypt;
using ReactiveUI;

namespace CardPile.Application.Models;

internal class SkeletonCardEntryModel : CardDataModel, ISkeletonCardEntryService
{
    internal SkeletonCardEntryModel(ICardData cardData, CardEntry cardEntry) : base(cardData)
    {
        CardEntry = cardEntry;
    }

    public CardEntry CardEntry { get; init; }

    public Range Range { get => CardEntry.Range; }

    public ImportanceLevel Importance { get => CardEntry.Importance; }

    public int Count { get => CardEntry.Count; }

    public bool IsSatisfied { get => CardEntry.IsSatisfied; }

    internal void NotifyPropertiesChanged()
    {
        this.RaisePropertyChanged(nameof(Count));
        this.RaisePropertyChanged(nameof(IsSatisfied));
    }
}
