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

    internal void UpdateCountVisibility(bool visible)
    {
        showCounts = visible;
        UpdateRangeText();
    }

    private void UpdateRangeText()
    {
        const string GREEN_CHECKMARK = "\u2705 ";

        if(showCounts)
        {
            if (SkeletonCardEntryService.Range != null)
            {
                RangeText = string.Format(
                    "{0}{1}{2} out of {3}",
                    SkeletonCardEntryService.IsSatisfied && SkeletonCardEntryService.Count > 0 ? GREEN_CHECKMARK : string.Empty,
                    ImportanceUtils.ToMarker(SkeletonCardEntryService.Importance),
                    SkeletonCardEntryService.Count,
                    SkeletonCardEntryService.Range.TextValue
                );
            }
            else
            {
                RangeText = string.Format(
                    "{0}{1}Currently {2}",
                    SkeletonCardEntryService.IsSatisfied && SkeletonCardEntryService.Count > 0 ? GREEN_CHECKMARK : string.Empty,
                    ImportanceUtils.ToMarker(SkeletonCardEntryService.Importance),
                    SkeletonCardEntryService.Count
                );
            }
        }
        else
        {
            if(SkeletonCardEntryService.Range != null)
            {
                RangeText = string.Format(
                    "{0}{1}",
                    ImportanceUtils.ToMarker(SkeletonCardEntryService.Importance),
                    SkeletonCardEntryService.Range.TextValue
                );
            }
            else
            {
                RangeText = string.Format(
                    "{0}Any",
                    ImportanceUtils.ToMarker(SkeletonCardEntryService.Importance)
                );
            }
        }
    }

    private string rangeText = string.Empty;

    private bool showCounts = true;
}
