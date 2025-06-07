using CardPile.Application.Services;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace CardPile.Application.ViewModels;

abstract internal class CardCollectionViewModel : ViewModelBase
{
    internal CardCollectionViewModel(Func<ICardDataService, int, CardDataViewModel> factory, Action<ObservableCollection<CardDataViewModel>> sorter)
    {
        Factory = factory;
        Sorter = sorter;
    }

    internal ObservableCollection<CardDataViewModel> Cards { get; } = [];

    internal void UpdateMetricVisibility(int metricIndex, bool visible)
    {
        foreach (var card in Cards)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }
    }

    internal void Update(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                Cards.Clear();
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                AddCards(e.NewItems);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                RemoveCards(e.OldItems);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                RemoveCards(e.OldItems);
                AddCards(e.NewItems);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                // NOOP
                break;
        }

        Sort();

        this.RaisePropertyChanged(nameof(Cards));
    }

    internal void Clear()
    {
        Cards.Clear();
    }

    internal void Sort()
    {
        Sorter(Cards);
    }

    internal abstract void AddCards(IList? items);

    internal abstract void RemoveCards(IList? items);

    protected Func<ICardDataService, int, CardDataViewModel> Factory { get; init; }

    protected Action<ObservableCollection<CardDataViewModel>> Sorter { get; init; }
}
