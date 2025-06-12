using CardPile.Application.Services;
using CardPile.CardData.Importance;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardPile.Application.ViewModels;

internal class SkeletonCardGroupViewModel : ViewModelBase
{
    internal SkeletonCardGroupViewModel(ISkeletonCardGroupService skeletonCardGroupService)
    {
        SkeletonCardGroupService = skeletonCardGroupService;

        Groups = [.. skeletonCardGroupService.Groups.Select(g => new SkeletonCardGroupViewModel(g))];

        Cards = [.. skeletonCardGroupService.Cards.Select((c, i) => new SkeletonCardEntryViewModel(c, i))];

        skeletonCardGroupService.ObservableForProperty(x => x.Count).Subscribe(_ => UpdateTitle());
        skeletonCardGroupService.ObservableForProperty(x => x.IsSatisfied).Subscribe(_ => UpdateTitle());

        UpdateTitle();
    }

    internal ISkeletonCardGroupService SkeletonCardGroupService { get; init; }

    internal string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    internal ObservableCollection<SkeletonCardGroupViewModel> Groups { get; init; }

    internal ObservableCollection<SkeletonCardEntryViewModel> Cards { get; init; }

    internal void UpdateCardMetricVisibility(Action<CardDataViewModel> updater)
    {
        foreach (var group in Groups)
        {
            group.UpdateCardMetricVisibility(updater);
        }

        foreach (var card in Cards)
        {
            updater(card);
        }
    }

    internal void UpdateCountVisibility(bool visible)
    {
        foreach (var group in Groups)
        {
            group.UpdateCountVisibility(visible);
        }

        foreach (var card in Cards)
        {
            card.UpdateCountVisibility(visible);
        }

        showCounts = visible;
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        const string GREEN_CHECKMARK = "\u2705 ";

        if(showCounts)
        {
            if (SkeletonCardGroupService.Range != null)
            {
                Title = string.Format(
                    "{0}{1}{2} ({3} out of {4})",
                    SkeletonCardGroupService.IsSatisfied && SkeletonCardGroupService.Count > 0 ? GREEN_CHECKMARK : string.Empty,
                    ImportanceUtils.ToMarker(SkeletonCardGroupService.Importance),
                    SkeletonCardGroupService.Name,
                    SkeletonCardGroupService.Count,
                    SkeletonCardGroupService.Range.TextValue);
            }
            else
            {
                Title = string.Format(
                    "{0}{1}{2} (currently {3})",
                    SkeletonCardGroupService.IsSatisfied && SkeletonCardGroupService.Count > 0 ? GREEN_CHECKMARK : string.Empty,
                    ImportanceUtils.ToMarker(SkeletonCardGroupService.Importance),
                    SkeletonCardGroupService.Name,
                    SkeletonCardGroupService.Count);
            }
        }
        else
        {
            if (SkeletonCardGroupService.Range != null)
            {
                Title = string.Format(
                    "{0}{1} ({2})",
                    ImportanceUtils.ToMarker(SkeletonCardGroupService.Importance),
                    SkeletonCardGroupService.Name,
                    SkeletonCardGroupService.Range.TextValue);
            }
            else
            {
                Title = string.Format(
                    "{0}{1}",
                    ImportanceUtils.ToMarker(SkeletonCardGroupService.Importance),
                    SkeletonCardGroupService.Name);
            }
        }
    }

    private string title = string.Empty;

    private bool showCounts = true;
}
