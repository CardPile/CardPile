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
        statisticsService = model.StatisticsService;
        metricDescriptionViewModels = [];
        sortByMetricDescriptionViewModel = null;
        statisticsViewModels = [];

        cardsInPackService.CardsInPack.CollectionChanged += UpdateCardsInPack;
        cardsInPackService.CardsMissingFromPack.CollectionChanged += UpdateCardsMissingFromPack;
        cardsInPackService.CardsUpcomingAfterPack.CollectionChanged += UpdateCardsUpcomingAfterPack;
        cardsInPackService.CardsSeen.CollectionChanged += UpdateCardsSeen;
        cardsInPackService.CardsInDeck.CollectionChanged += UpdateDeck;
        cardsInPackService.ObservableForProperty(x => x.PreviousPick)
                          .Subscribe(x => UpdatePreviouslyPickedCardFromPack(x.Value));

        statisticsService.Statistics.CollectionChanged += UpdateStatistics;

        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder, true)
                                                    .Subscribe(p => ClearMetricViewModels());
        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder, true)
                                                    .Subscribe(p => ClearStatisticsViewModels());
        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder, true)
                                                    .Subscribe(p => ClearCardData());
        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder)
                                                    .Subscribe(p => UpdateMetricViewModels(p.Value));

        model.ObservableForProperty(p => p.IsCardDataSourceBeingBuilt)
             .Subscribe(p => IsCardDataSourceBeingBuilt = p.Value);

        UpdateMetricViewModels(cardDataSourceBuilderCollectionService.CurrentCardDataSourceBuilder);

        InitializeStatistics();

        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(p => SortCardsDescendingBySelectedMetric(CardsInPack));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(p => SortCardsDescendingBySelectedMetric(WhiteCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(p => SortCardsDescendingBySelectedMetric(BlueCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(p => SortCardsDescendingBySelectedMetric(BlackCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(p => SortCardsDescendingBySelectedMetric(RedCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(p => SortCardsDescendingBySelectedMetric(GreenCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(p => SortCardsDescendingBySelectedMetric(MulticolorCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(p => SortCardsDescendingBySelectedMetric(ColorlessCardsSeen));

        logWindow = new LogWindow()
        {
            DataContext = new LogWindowViewModel(model.LogService)
        };

        ShowLogCommand = ReactiveCommand.Create(() =>
        {
            if(!logWindow.IsVisible)
            {
                logWindow.Show();
            }
            else
            {
                logWindow.Activate();
            }
        });

        ShowCardDataSourceSettingsDialog = new Interaction<CardDataSourceSettingsDialogViewModel, bool>();

        ShowCardDataSourceSettingsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var settingsViewModel = new CardDataSourceSettingsDialogViewModel(cardDataSourceBuilderCollectionService.CurrentCardDataSourceBuilder.Settings);
            var result = await ShowCardDataSourceSettingsDialog.Handle(settingsViewModel);
            if (result)
            {
                settingsViewModel.ApplyChanges();
            }
            else
            {
                settingsViewModel.DiscardChanges();
            }
        });

        ShowSettingsCommand = ReactiveCommand.Create(() =>
        {
            // TODO....
        });

        ClearCardsSeenAndDeckCommand = ReactiveCommand.Create(() =>
        {
            cardsInPackService.ClearCardsSeenAndDeck();
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

    internal ObservableCollection<CardDataSourceStatisticViewModel> Statistics
    {
        get => statisticsViewModels;
        set => this.RaiseAndSetIfChanged(ref statisticsViewModels, value);
    }

    internal ObservableCollection<CardDataViewModel> CardsInPack { get; } = [];

    internal ObservableCollection<CardViewModel> CardsMissingFromPack { get; } = [];

    internal ObservableCollection<CardViewModel> CardsUpcomingAfterPack { get; } = [];

    internal ObservableCollection<CardDataViewModel> WhiteCardsSeen { get; } = [];
    internal ObservableCollection<CardDataViewModel> BlueCardsSeen { get; } = [];
    internal ObservableCollection<CardDataViewModel> BlackCardsSeen { get; } = [];
    internal ObservableCollection<CardDataViewModel> RedCardsSeen { get; } = [];
    internal ObservableCollection<CardDataViewModel> GreenCardsSeen { get; } = [];
    internal ObservableCollection<CardDataViewModel> MulticolorCardsSeen { get; } = [];
    internal ObservableCollection<CardDataViewModel> ColorlessCardsSeen { get; } = [];

    internal ObservableCollection<CardViewModel> CardsInDeck { get; } = [];

    internal CardViewModel? PreviouslyPickedCardFromPack
    {
        get => previouslyPickedCardFromPack;
        private set => this.RaiseAndSetIfChanged(ref previouslyPickedCardFromPack, value);
    }

    internal ICommand ShowLogCommand { get; init; }

    internal Interaction<CardDataSourceSettingsDialogViewModel, bool> ShowCardDataSourceSettingsDialog { get; }

    internal ICommand ShowCardDataSourceSettingsCommand { get; init; }

    internal ICommand ShowSettingsCommand { get; init; }

    internal ICommand ClearCardsSeenAndDeckCommand { get; init; }

    internal bool IsCardDataSourceBeingBuilt
    {
        get => isCardDataSourceBeingBuilt;
        private set => this.RaiseAndSetIfChanged(ref isCardDataSourceBeingBuilt, value);
    }

    private static void SortCards<T, TKey>(ObservableCollection<T> collection, Func<T, TKey> selector, IComparer<TKey>? comparer = null)
    {
        List<T> sorted = [.. collection.OrderBy(selector, comparer ?? Comparer<TKey>.Default)];
        for (int i = 0; i < sorted.Count; i++)
        {
            collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }

    private static void SortCardsDescending<T, TKey>(ObservableCollection<T> collection, Func<T, TKey> selector, IComparer<TKey>? comparer = null)
    {
        List<T> sorted = [.. collection.OrderByDescending(selector, comparer ?? Comparer<TKey>.Default)];
        for (int i = 0; i < sorted.Count; i++)
        {
            collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }

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
        SortCardsDescendingBySelectedMetric(CardsInPack);
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
                        var newCardVm = new CardViewModel(cardDataService);
                        CardsUpcomingAfterPack.Add(newCardVm);
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

    private void UpdateCardsSeen(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void processNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        var newCardVm = new CardDataViewModel(cardDataService);
                        ClearCardMetricVisibility(newCardVm);
                        if (cardDataService.Colors.Count == 0)
                        {
                            ColorlessCardsSeen.Add(newCardVm);
                        }
                        else if (cardDataService.Colors.Count == 1)
                        {
                            if(cardDataService.Colors.First() == Color.White)
                            {
                                WhiteCardsSeen.Add(newCardVm);
                            }
                            else if(cardDataService.Colors.First() == Color.Blue)
                            {
                                BlueCardsSeen.Add(newCardVm);
                            }
                            else if (cardDataService.Colors.First() == Color.Black)
                            {
                                BlackCardsSeen.Add(newCardVm);
                            }
                            else if (cardDataService.Colors.First() == Color.Red)
                            {
                                RedCardsSeen.Add(newCardVm);
                            }
                            else if (cardDataService.Colors.First() == Color.Green)
                            {
                                GreenCardsSeen.Add(newCardVm);
                            }
                        }
                        else
                        {
                            MulticolorCardsSeen.Add(newCardVm);
                        }
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
                        if (cardDataService.Colors.Count == 0)
                        {
                            ColorlessCardsSeen.Remove(ColorlessCardsSeen.Where(x => x.CardDataService == item));
                        }
                        else if (cardDataService.Colors.Count == 1)
                        {
                            if (cardDataService.Colors.First() == Color.White)
                            {
                                WhiteCardsSeen.Remove(WhiteCardsSeen.Where(x => x.CardDataService == item));
                            }
                            else if (cardDataService.Colors.First() == Color.Blue)
                            {
                                BlueCardsSeen.Remove(BlueCardsSeen.Where(x => x.CardDataService == item));
                            }
                            else if (cardDataService.Colors.First() == Color.Black)
                            {
                                BlackCardsSeen.Remove(BlackCardsSeen.Where(x => x.CardDataService == item));
                            }
                            else if (cardDataService.Colors.First() == Color.Red)
                            {
                                RedCardsSeen.Remove(RedCardsSeen.Where(x => x.CardDataService == item));
                            }
                            else if (cardDataService.Colors.First() == Color.Green)
                            {
                                GreenCardsSeen.Remove(GreenCardsSeen.Where(x => x.CardDataService == item));
                            }
                        }
                        else
                        {
                            MulticolorCardsSeen.Remove(MulticolorCardsSeen.Where(x => x.CardDataService == item));
                        }
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
            WhiteCardsSeen.Clear();
            BlueCardsSeen.Clear();
            BlackCardsSeen.Clear();
            RedCardsSeen.Clear();
            GreenCardsSeen.Clear();
            MulticolorCardsSeen.Clear();
            ColorlessCardsSeen.Clear();
        }

        DispatchObservableCardCollection(e, clearItems, processNewItems, processOldItems);
        SortCardsDescendingBySelectedMetric(WhiteCardsSeen);
        SortCardsDescendingBySelectedMetric(BlueCardsSeen);
        SortCardsDescendingBySelectedMetric(BlackCardsSeen);
        SortCardsDescendingBySelectedMetric(RedCardsSeen);
        SortCardsDescendingBySelectedMetric(GreenCardsSeen);
        SortCardsDescendingBySelectedMetric(MulticolorCardsSeen);
        SortCardsDescendingBySelectedMetric(ColorlessCardsSeen);
    }

    private void UpdateDeck(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
                        CardsInDeck.Add(newCardDataVm);
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
                        CardsInDeck.Remove(CardsInDeck.Where(x => x.CardDataService == item));
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
            CardsInDeck.Clear();
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

    private void InitializeStatistics()
    {
        foreach (var statisticService in statisticsService.Statistics)
        {
            Statistics.Add(new CardDataSourceStatisticViewModel(statisticService));
        }
    }

    private void UpdateStatistics(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void processNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ICardDataSourceStatisticService statisticService)
                    {
                        Statistics.Add(new CardDataSourceStatisticViewModel(statisticService));
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataSourceStatisticService as a new item");
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
                    if (item is ICardDataSourceStatisticService statisticService)
                    {
                        Statistics.Remove(Statistics.Where(x => x.CardDataSourceStatisticService == item));
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataSourceStatisticService as a old item");
                    }
                }
            }
        }

        void clearItems()
        {
            Statistics.Clear();
        }

        DispatchObservableCardCollection(e, clearItems, processNewItems, processOldItems);
    }

    private void ClearMetricViewModels()
    {
        metricDescriptionViewModels.Clear();
        SortByMetricDescription = null;
    }

    private void ClearStatisticsViewModels()
    {
        statisticsViewModels.Clear();
    }

    private void ClearCardData()
    {
        CardsInPack.Clear();
        CardsMissingFromPack.Clear();
        CardsUpcomingAfterPack.Clear();
        WhiteCardsSeen.Clear();
        BlueCardsSeen.Clear();
        BlackCardsSeen.Clear();
        RedCardsSeen.Clear();
        GreenCardsSeen.Clear();
        MulticolorCardsSeen.Clear();
        ColorlessCardsSeen.Clear();
        CardsInDeck.Clear();
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

    private void SortCardsDescendingBySelectedMetric(ObservableCollection<CardDataViewModel> collection)
    {
        if (sortByMetricDescriptionViewModel == null)
        {
            return;
        }

        IComparer<ICardMetric> internalComparer = sortByMetricDescriptionViewModel.Comparer;
        int metricIndex = metricDescriptionViewModels.IndexOf(sortByMetricDescriptionViewModel);

        CardDataViewModelComparer comparer = new CardDataViewModelComparer(internalComparer, metricIndex);
        SortCardsDescending(collection, x => x, comparer);
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

    private void ClearCardMetricVisibility(CardDataViewModel cardVm)
    {
        for (int i = 0; i < metricDescriptionViewModels.Count; ++i)
        {
            cardVm.Metrics[i].Visible = false;
        }
    }

    private ICardDataSourceBuilderCollectionService cardDataSourceBuilderCollectionService;
    private ICardsInPackService cardsInPackService;
    private ICardDataSourceStatisticsService statisticsService;

    private ObservableCollection<CardDataMetricDescriptionViewModel> metricDescriptionViewModels;
    private CardDataMetricDescriptionViewModel? sortByMetricDescriptionViewModel;

    private ObservableCollection<CardDataSourceStatisticViewModel> statisticsViewModels;

    private CardViewModel? previouslyPickedCardFromPack = null;

    private LogWindow logWindow;

    private bool isCardDataSourceBeingBuilt = false;
}
