using CardPile.App.Models;
using CardPile.App.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System;
using System.Reactive.Linq;
using System.Linq;
using DynamicData;
using System.Collections;
using System.Collections.Generic;
using CardPile.CardData;
using System.Windows.Input;
using CardPile.App.Views;

namespace CardPile.App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel() : this(new CardPileModel())
    { }

    internal MainWindowViewModel(CardPileModel model)
    {
        cardDataSourceBuilderCollectionService = model.CardDataSourceBuilderCollectionService;
        cardsInPackService = model.CurrentCardsInPackService;
        metricDescriptionViewModels = [];
        sortByMetricDescriptionViewModel = null;

        cardsInPackService.CardsInPack.CollectionChanged += UpdateCardsInPack;
        cardsInPackService.CardsMissingFromPack.CollectionChanged += UpdateCardsMissingFromPack;
        cardsInPackService.CardsUpcomingAfterPack.CollectionChanged += UpdateCardsUpcomingAfterPack;
        cardsInPackService.ObservableForProperty(x => x.PreviousPick)
                          .Subscribe(x => UpdatePreviouslyPickedCardFromPack(x.Value));

        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder, true)
                                                    .Subscribe(p => ClearMetricViewModels());
        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder, true)
                                                    .Subscribe(p => ClearCardData());
        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder)
                                                    .Subscribe(p => UpdateMetricViewModels(p.Value));

        UpdateMetricViewModels(cardDataSourceBuilderCollectionService.CurrentCardDataSourceBuilder);

        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(p => SortCardsInPackBySelectedMetric());

        ShowLogCommand = ReactiveCommand.Create(() =>
        {
            LogWindow logWindow = new LogWindow()
            {
                DataContext = new LogWindowViewModel(model.LogService)
            };
            logWindow.Show();
        });
    }

    internal ICardDataSourceBuilderCollectionService CardDataSourceBuilderCollectionService
    {
        get => cardDataSourceBuilderCollectionService;
    }

    internal ObservableCollection<CardDataMetricDescriptionViewModel> MetricDescriptions
    {
        get => metricDescriptionViewModels;
        set => this.RaiseAndSetIfChanged(ref metricDescriptionViewModels, value);
    }

    internal CardDataMetricDescriptionViewModel? SortByMetricDescription
    {
        get => sortByMetricDescriptionViewModel;
        set => this.RaiseAndSetIfChanged(ref sortByMetricDescriptionViewModel, value);
    }

    internal ObservableCollection<CardDataViewModel> CardsInPack { get; } = [];

    internal ObservableCollection<CardViewModel> CardsMissingFromPack { get; } = [];

    internal ObservableCollection<CardViewModel> CardsUpcomingAfterPack { get; } = [];

    internal CardViewModel? PreviouslyPickedCardFromPack
    {
        get => previouslyPickedCardFromPack;
        private set => this.RaiseAndSetIfChanged(ref previouslyPickedCardFromPack, value);
    }

    internal ICommand ShowLogCommand { get; init; }

    private void UpdateCardsInPack(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void processNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        var newCardDataVm = new CardDataViewModel(cardDataService);
                        UpdateCardMetricVisibility(newCardDataVm);
                        CardsInPack.Add(newCardDataVm);
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataService as a new item");
                    }
                }
            }
        }

        void processOldItems(IList? oldItems)
        {
            if (oldItems != null)
            {
                foreach (var item in oldItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        CardsInPack.Remove(CardsInPack.Where(x => x.CardDataService == item));
                        var newCardDataVm = new CardDataViewModel(cardDataService);
                        UpdateCardMetricVisibility(newCardDataVm);
                        CardsInPack.Add(newCardDataVm);
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataService as a old item");
                    }
                }
            }
        }

        void clearItems()
        {
            CardsInPack.Clear();
        }

        DispatchObservableCardCollection(e, clearItems, processNewItems, processOldItems);
        SortCardsInPackBySelectedMetric();
    }

    private void UpdateCardsMissingFromPack(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void processNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        var newCardDataVm = new CardViewModel(cardDataService);
                        CardsMissingFromPack.Add(newCardDataVm);
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataService as a new item");
                    }
                }
            }
        }

        void processOldItems(IList? oldItems)
        {
            if (oldItems != null)
            {
                foreach (var item in oldItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        CardsMissingFromPack.Remove(CardsMissingFromPack.Where(x => x.CardDataService == item));
                        var newCardDataVm = new CardViewModel(cardDataService);
                        CardsMissingFromPack.Add(newCardDataVm);
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataService as a old item");
                    }
                }
            }
        }

        void clearItems()
        {
            CardsMissingFromPack.Clear();
        }

        DispatchObservableCardCollection(e, clearItems, processNewItems, processOldItems);
    }

    private void UpdateCardsUpcomingAfterPack(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void processNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        var newCardDataVm = new CardViewModel(cardDataService);
                        CardsUpcomingAfterPack.Add(newCardDataVm);
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataService as a new item");
                    }
                }
            }
        }

        void processOldItems(IList? oldItems)
        {
            if (oldItems != null)
            {
                foreach (var item in oldItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        CardsUpcomingAfterPack.Remove(CardsUpcomingAfterPack.Where(x => x.CardDataService == item));
                        var newCardDataVm = new CardViewModel(cardDataService);
                        CardsUpcomingAfterPack.Add(newCardDataVm);
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataService as a old item");
                    }
                }
            }
        }

        void clearItems()
        {
            CardsUpcomingAfterPack.Clear();
        }

        DispatchObservableCardCollection(e, clearItems, processNewItems, processOldItems);
    }
    
    private void DispatchObservableCardCollection(System.Collections.Specialized.NotifyCollectionChangedEventArgs e, Action clearItems, Action<IList?> processNewItems, Action<IList?> processOldItems)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                clearItems();
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                processNewItems(e.NewItems);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                processOldItems(e.OldItems);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                processOldItems(e.OldItems);
                processNewItems(e.NewItems);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                // NOOP
                break;
        }
    }

    private void UpdatePreviouslyPickedCardFromPack(ICardDataService? cardDataService)
    {
        PreviouslyPickedCardFromPack = cardDataService != null ? new CardViewModel(cardDataService, true) : null;
    }

    private void ClearMetricViewModels()
    {
        metricDescriptionViewModels.Clear();
        SortByMetricDescription = null;
    }

    private void ClearCardData()
    {
        CardsInPack.Clear();
        CardsMissingFromPack.Clear();
        PreviouslyPickedCardFromPack = null;
    }

    private void UpdateMetricViewModels(ICardDataSourceBuilderService builder)
    {
        metricDescriptionViewModels.Clear();
        SortByMetricDescription = null;

        foreach (ICardMetricDescriptionService cardMetricDescriptionModel in builder.MetricDescriptions)
        {
            var viewModel = new CardDataMetricDescriptionViewModel(cardMetricDescriptionModel);
            viewModel.ObservableForProperty(p => p.Visible)
                     .Subscribe(p => UpdateMetricVisibility(p.Sender, p.Value));

            metricDescriptionViewModels.Add(viewModel);

            if(cardMetricDescriptionModel.IsDefaultVisible)
            {
                viewModel.Visible = true;
            }

            if(cardMetricDescriptionModel.IsDefaultMetric)
            {
                SortByMetricDescription = viewModel;
            }
        }
    }

    private class CardDataViewModelComparer : IComparer<CardDataViewModel>
    {
        internal CardDataViewModelComparer(IComparer<ICardMetric> internalComparer, int metricIndex)
        {
            this.internalComparer = internalComparer;
            this.metricIndex = metricIndex;
        }

        public int Compare(CardDataViewModel? x, CardDataViewModel? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            var comparisonResult = internalComparer.Compare(x.Metrics[metricIndex].Metric, y.Metrics[metricIndex].Metric);
            if (comparisonResult == 0)
            {

                return x.CardDataService.ArenaCardId.CompareTo(y.CardDataService.ArenaCardId);
            }

            return comparisonResult;
        }

        private IComparer<ICardMetric> internalComparer;
        private int metricIndex;
    };

    private void SortCardsInPackBySelectedMetric()
    {
        if (sortByMetricDescriptionViewModel == null)
        {
            return;
        }

        IComparer<ICardMetric> internalComparer = sortByMetricDescriptionViewModel.Comparer;
        int metricIndex = metricDescriptionViewModels.IndexOf(sortByMetricDescriptionViewModel);

        CardDataViewModelComparer comparer = new CardDataViewModelComparer(internalComparer, metricIndex);
        List<CardDataViewModel> sorted = [.. CardsInPack.OrderByDescending(x => x, comparer)];
        for (int i = 0; i < sorted.Count; i++)
        {
            CardsInPack.Move(CardsInPack.IndexOf(sorted[i]), i);
        }
    }

    private void UpdateMetricVisibility(CardDataMetricDescriptionViewModel vm, bool visible)
    {
        int metricIndex = metricDescriptionViewModels.IndexOf(vm);
        foreach (var card in CardsInPack)
        {
            card.Metrics[metricIndex].Visible = visible;
        }
    }

    private void UpdateCardMetricVisibility(CardDataViewModel cardVm)
    {
        for(int i = 0; i < metricDescriptionViewModels.Count; ++i)
        {
            cardVm.Metrics[i].Visible = metricDescriptionViewModels[i].Visible;
        }
    }

    private ICardDataSourceBuilderCollectionService cardDataSourceBuilderCollectionService;
    private ICardsInPackService cardsInPackService;

    private ObservableCollection<CardDataMetricDescriptionViewModel> metricDescriptionViewModels;
    private CardDataMetricDescriptionViewModel? sortByMetricDescriptionViewModel;

    private CardViewModel? previouslyPickedCardFromPack = null;
}
