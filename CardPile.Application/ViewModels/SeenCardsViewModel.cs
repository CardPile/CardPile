using CardPile.Application.Services;
using CardPile.CardData;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardPile.Application.ViewModels;

internal class SeenCardsViewModel : ViewModelBase
{
    internal SeenCardsViewModel(Func<ICardDataService, int, CardDataViewModel> factory, Action<ObservableCollection<CardDataViewModel>> sorter)
    {
        ColorStacks = new ColorStacksViewModel(factory, sorter);

        Packs = [];

        this.factory = factory;
        this.sorter = sorter;
    }

    internal ColorStacksViewModel ColorStacks { get; init; }

    internal ObservableCollection<SeenPackViewModel> Packs { get; init; }

    internal void UpdateCardsSeen(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        ColorStacks.Update(sender, e);
    }

    internal void UpdatePacksSeen(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                Packs.Clear();
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                AddPacks(e.NewItems);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                RemovePacks(e.OldItems);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                RemovePacks(e.OldItems);
                AddPacks(e.NewItems);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                // NOOP
                break;
        }

        this.RaisePropertyChanged(nameof(Packs));
    }

    internal void Clear()
    {
        ColorStacks.Clear();

        Packs.Clear();
    }

    internal void Sort()
    {
        ColorStacks.Sort();

        foreach (var pack in Packs)
        {
            pack.Sort();
        }
    }

    internal void UpdateMetricVisibility(int metricIndex, bool visible)
    {
        ColorStacks.UpdateMetricVisibility(metricIndex, visible);

        foreach (var pack in Packs)
        {
            pack.UpdateMetricVisibility(metricIndex, visible);
        }
    }

    private void AddPacks(IList? items)
    {
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item is IDraftPackService draftPackService)
            {
                Packs.Add(new SeenPackViewModel(draftPackService, factory, sorter));
            }
            else
            {
                throw new InvalidOperationException("Expected a IDraftPackService as a new item");
            }
        }
    }

    private void RemovePacks(IList? items)
    {
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item is IDraftPackService draftPackService)
            {
                Packs.Remove(Packs.Where(x => x.DraftPackService == item));
            }
            else
            {
                throw new InvalidOperationException("Expected a IDraftPackService as a old item");
            }
        }
    }

    private Func<ICardDataService, int, CardDataViewModel> factory;
    private Action<ObservableCollection<CardDataViewModel>> sorter;
}
