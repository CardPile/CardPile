using Avalonia.Data.Converters;
using CardPile.Application.Models;
using CardPile.Application.Services;
using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardPile.Application.ViewModels;

internal class CardStackViewModel : CardCollectionViewModel
{
    public static FuncValueConverter<ICollection<CardDataViewModel>, int> CardCollectionToStackHeightConverter { get; } = new FuncValueConverter<ICollection<CardDataViewModel>, int>(collection =>
    {
        if (collection == null || collection.Count == 0)
        {
            return 0;
        }

        return (collection.Count - 1) * CardDataModel.CARD_HEADER_SIZE + CardDataModel.CARD_IMAGE_HEIGHT;
    });

    internal CardStackViewModel(
        Func<ICardDataService, bool> predicate,
        Func<ICardDataService, int, CardDataViewModel> factory,
        Action<ObservableCollection<CardDataViewModel>> sorter) : base(factory, sorter)
    {
        this.predicate = predicate;
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
                if (predicate(cardDataService))
                {
                    Cards.Add(Factory(cardDataService, Cards.Count));
                }
            }
            else
            {
                throw new InvalidOperationException("Expected a ICardDataService as an added item");
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
            if (item is ICardDataService cardDataService)
            {
                if (predicate(cardDataService))
                {
                    Cards.Remove(Cards.Where(x => x.CardDataService == item));
                }
            }
            else
            {
                throw new InvalidOperationException("Expected a ICardDataService as an removed item");
            }
        }
    }

    private Func<ICardDataService, bool> predicate;
}
