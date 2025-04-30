using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CardPile.Application.ViewModels;

internal class CardListViewModel : ViewModelBase, ICollection<CardDataViewModel>
{
    internal CardListViewModel()
    {
        Cards = [];
    }

    internal CardListViewModel(IEnumerable<CardDataViewModel> e)
    {
        Cards = [.. e];
    }

    internal CardListViewModel(ReadOnlySpan<CardDataViewModel> e)
    {
        Cards = [.. e];
    }

    internal ObservableCollection<CardDataViewModel> Cards { get; }

    public int Count => Cards.Count;

    public bool IsReadOnly => false;

    public IEnumerator<CardDataViewModel> GetEnumerator()
    {
        return Cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(CardDataViewModel item)
    {
        Cards.Add(item);
    }

    public void Clear()
    {
        Cards.Clear();
    }

    public bool Contains(CardDataViewModel item)
    {
        return Cards.Contains(item);
    }

    public void CopyTo(CardDataViewModel[] array, int arrayIndex)
    {
        Cards.CopyTo(array, arrayIndex);
    }

    public bool Remove(CardDataViewModel item)
    {
        return Cards.Remove(item);
    }

    internal static CardListViewModel Create(ReadOnlySpan<CardDataViewModel> values) => new(values);
}
