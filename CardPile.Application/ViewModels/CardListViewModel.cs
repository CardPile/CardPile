using CardPile.Application.Services;
using System.Collections.ObjectModel;
using System;
using System.Collections;
using DynamicData;
using System.Collections.Generic;
using System.Linq;

namespace CardPile.Application.ViewModels;

internal class CardListViewModel : CardCollectionViewModel
{
    internal CardListViewModel(Func<ICardDataService, int, CardDataViewModel> factory,
                               Action<ObservableCollection<CardDataViewModel>> sorter) : base(factory, sorter)
    {
    }

    internal override void AddCards(IList? items)
    {
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item is ICardDataService cardDataService)
            {
                Cards.Add(Factory(cardDataService, Cards.Count));
            }
            else
            {
                throw new InvalidOperationException("Expected a ICardDataService as a added item");
            }
        }
    }

    internal override void RemoveCards(IList? items)
    {
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item is ICardDataService)
            {
                Cards.Remove(Cards.Where(x => x.CardDataService == item));
            }
            else
            {
                throw new InvalidOperationException("Expected a ICardDataService as a removed item");
            }
        }
    }
}
