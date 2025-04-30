using CardPile.Application.Services;
using CardPile.CardData.Importance;
using ReactiveUI;

namespace CardPile.Application.ViewModels;

internal class SkeletonCardEntryViewModel : CardDataViewModel
{
    internal SkeletonCardEntryViewModel(ISkeletonCardEntryService skeletonCardEntryService, int index) : base(skeletonCardEntryService, index)
    {
        SkeletonCardEntryService = skeletonCardEntryService;

        UpdateRangeText();
    }

    internal ISkeletonCardEntryService SkeletonCardEntryService { get; init; }

    internal string RangeText 
    { 
        get => rangeText;
        set => this.RaiseAndSetIfChanged(ref rangeText, value);
    }

    private void UpdateRangeText()
    {
        RangeText = string.Format(
            "{0}{1}",
            ImportanceUtils.ToMarker(SkeletonCardEntryService.Importance),
            SkeletonCardEntryService.Range.TextValue
        );
    }

    private string rangeText = string.Empty;
}
