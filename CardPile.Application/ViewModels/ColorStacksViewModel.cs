using CardPile.Application.Services;
using CardPile.CardData;
using CsvHelper;
using System.Collections.ObjectModel;
using System;
using System.Collections;

namespace CardPile.Application.ViewModels;

internal class ColorStacksViewModel : ViewModelBase
{
    internal ColorStacksViewModel(Func<ICardDataService, int, CardDataViewModel> factory, Action<ObservableCollection<CardDataViewModel>> sorter)
    {
        WhiteCards = new(WhiteCardPredicate, factory, sorter);
        BlueCards = new(BlueCardPredicate, factory, sorter);
        BlackCards = new(BlackCardPredicate, factory, sorter);
        RedCards = new(RedCardPredicate, factory, sorter);
        GreenCards = new(GreenCardPredicate, factory, sorter);
        MulticolorCards = new(MulticolorCardPredicate, factory, sorter);
        ColorlessCards = new(ColorlessCardPredicate, factory, sorter);
    }

    internal CardStackViewModel WhiteCards { get; }

    internal CardStackViewModel BlueCards { get; }

    internal CardStackViewModel BlackCards { get; }

    internal CardStackViewModel RedCards { get; }

    internal CardStackViewModel GreenCards { get; }

    internal CardStackViewModel MulticolorCards { get; }

    internal CardStackViewModel ColorlessCards { get; }

    internal void AddCards(IList? cards)
    {
        WhiteCards.AddCards(cards);
        BlueCards.AddCards(cards);
        BlackCards.AddCards(cards);
        RedCards.AddCards(cards);
        GreenCards.AddCards(cards);
        MulticolorCards.AddCards(cards);
        ColorlessCards.AddCards(cards);
    }

    internal void Update(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        WhiteCards.Update(sender, e);
        BlueCards.Update(sender, e);
        BlackCards.Update(sender, e);
        RedCards.Update(sender, e);
        GreenCards.Update(sender, e);
        MulticolorCards.Update(sender, e);
        ColorlessCards.Update(sender, e);
    }

    internal void Clear()
    {
        WhiteCards.Clear();
        BlueCards.Clear();
        BlackCards.Clear();
        RedCards.Clear();
        GreenCards.Clear();
        MulticolorCards.Clear();
        ColorlessCards.Clear();
    }

    internal void Sort()
    {
        WhiteCards.Sort();
        BlueCards.Sort();
        BlackCards.Sort();
        RedCards.Sort();
        GreenCards.Sort();
        MulticolorCards.Sort();
        ColorlessCards.Sort();
    }

    internal void UpdateMetricVisibility(int metricIndex, bool visible)
    {
        WhiteCards.UpdateMetricVisibility(metricIndex, visible);
        BlueCards.UpdateMetricVisibility(metricIndex, visible);
        BlackCards.UpdateMetricVisibility(metricIndex, visible);
        RedCards.UpdateMetricVisibility(metricIndex, visible);
        GreenCards.UpdateMetricVisibility(metricIndex, visible);
        MulticolorCards.UpdateMetricVisibility(metricIndex, visible);
        ColorlessCards.UpdateMetricVisibility(metricIndex, visible);
    }

    private static bool WhiteCardPredicate(ICardDataService cardDataService)
    {
        return cardDataService.Colors == Color.White;
    }

    private static bool BlueCardPredicate(ICardDataService cardDataService)
    {
        return cardDataService.Colors == Color.Blue;
    }

    private static bool BlackCardPredicate(ICardDataService cardDataService)
    {
        return cardDataService.Colors == Color.Black;
    }

    private static bool GreenCardPredicate(ICardDataService cardDataService)
    {
        return cardDataService.Colors == Color.Green;
    }

    private static bool RedCardPredicate(ICardDataService cardDataService)
    {
        return cardDataService.Colors == Color.Red;
    }

    private static bool MulticolorCardPredicate(ICardDataService cardDataService)
    {
        return cardDataService.Colors.Count() > 1;
    }

    private static bool ColorlessCardPredicate(ICardDataService cardDataService)
    {
        return cardDataService.Colors.Count() == 0;
    }
}
