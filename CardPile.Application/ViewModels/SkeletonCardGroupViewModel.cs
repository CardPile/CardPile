using CardPile.Application.Services;
using CardPile.CardData.Importance;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace CardPile.Application.ViewModels;

internal class SkeletonCardGroupViewModel : ViewModelBase
{
    internal SkeletonCardGroupViewModel(ISkeletonCardGroupService skeletonCardGroupService)
    {
        SkeletonCardGroupService = skeletonCardGroupService;

        Groups = [.. skeletonCardGroupService.Groups.Select(g => new SkeletonCardGroupViewModel(g))];

        Cards = [.. skeletonCardGroupService.Cards.Select((c, i) => new SkeletonCardEntryViewModel(c, i))];

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

    private void UpdateTitle()
    {
        if(SkeletonCardGroupService.Range != null)
        {
            Title = string.Format(
                "{0}{1} ({2})",
                ImportanceUtils.ToMarker(SkeletonCardGroupService.Importance),
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SkeletonCardGroupService.Name),
                SkeletonCardGroupService.Range.TextValue);
        }
        else
        {
            Title = string.Format(
                "{0}{1}",
                ImportanceUtils.ToMarker(SkeletonCardGroupService.Importance),
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SkeletonCardGroupService.Name));
        }
    }

    private string title = string.Empty;
}
