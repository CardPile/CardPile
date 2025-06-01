using CardPile.Application.Services;
using System.Collections.ObjectModel;
using System;

namespace CardPile.Application.ViewModels;

internal class SeenPackViewModel : ViewModelBase
{
    internal SeenPackViewModel(IDraftPackService draftPackService, Func<ICardDataService, int, CardDataViewModel> factory, Action<ObservableCollection<CardDataViewModel>> sorter)
    {
        DraftPackService = draftPackService;
        
        ColorStacks = new ColorStacksViewModel(factory, sorter);
        ColorStacks.AddCards(draftPackService.Cards);

        Cards = new CardListViewModel(factory, sorter);
        Cards.AddCards(draftPackService.Cards);
    }

    internal IDraftPackService DraftPackService { get; init; }

    internal int PackNumber { get => DraftPackService.PackNumber; }

    internal int PickNumber { get => DraftPackService.PickNumber; }

    internal ColorStacksViewModel ColorStacks { get; init; }

    internal CardListViewModel Cards { get; init; }

    internal void Clear()
    {
        ColorStacks.Clear();
    }

    internal void Sort()
    {
        ColorStacks.Sort();
    }

    internal void UpdateMetricVisibility(int metricIndex, bool visible)
    {
        ColorStacks.UpdateMetricVisibility(metricIndex, visible);
    }
}
