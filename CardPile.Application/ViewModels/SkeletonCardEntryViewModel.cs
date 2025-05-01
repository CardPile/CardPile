using CardPile.Application.Services;
using CardPile.CardData.Importance;
using ReactiveUI;
using System;

namespace CardPile.Application.ViewModels;

internal class SkeletonCardEntryViewModel : CardDataViewModel
{
    internal SkeletonCardEntryViewModel(ISkeletonCardEntryService skeletonCardEntryService, int index) : base(skeletonCardEntryService, index)
    {
        SkeletonCardEntryService = skeletonCardEntryService;

        skeletonCardEntryService.ObservableForProperty(x => x.Count).Subscribe(_ => UpdateRangeText());
        skeletonCardEntryService.ObservableForProperty(x => x.IsSatisfied).Subscribe(_ => UpdateRangeText());

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
        const string GREEN_CHECKMARK = "\u2705 ";

        RangeText = string.Format(
            "{0}{1} out of {2}{3}",
            SkeletonCardEntryService.IsSatisfied && SkeletonCardEntryService.Count > 0 ? ConverterUtils.HIGHLIGHT_GREEN_MARKER : ImportanceUtils.ToMarker(SkeletonCardEntryService.Importance),
            SkeletonCardEntryService.Count,
            SkeletonCardEntryService.Range.TextValue,
            SkeletonCardEntryService.IsSatisfied ? GREEN_CHECKMARK : string.Empty
        );
    }

    private string rangeText = string.Empty;
}
