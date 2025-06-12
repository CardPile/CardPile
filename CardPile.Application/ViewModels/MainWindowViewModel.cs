using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using CardPile.Application.Models;
using CardPile.Application.Services;
using CardPile.Application.Views;
using CardPile.CardData;
using DynamicData;
using ReactiveUI;

namespace CardPile.Application.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel() : this(new CardPileModel())
    { }

    internal MainWindowViewModel(CardPileModel model)
    {
        cardDataSourceBuilderCollectionService = model.CardDataSourceBuilderCollectionService;
        cardsInPackService = model.CurrentCardsInPackService;
        statisticsService = model.StatisticsService;
        cryptService = model.CryptService;
        metricDescriptionViewModels = [];
        sortByMetricDescriptionViewModel = null;
        statisticsViewModels = [];

        CardsInPack = new CardListViewModel((c, i) => MakeCardDataViewModel(c, i), SortCardsDescendingBySelectedMetric);
        SeenCards = new SeenCardsViewModel((c, i) => MakeCardDataViewModel(c, i), SortCardsDescendingBySelectedMetric);
        CardsMissingFromPack = new CardListViewModel((c, i) => MakeCardDataViewModel(c, i), SortCardsDescendingBySelectedMetric);
        CardsUpcomingAfterPack = new CardListViewModel((c, i) => MakeCardDataViewModel(c, i), SortCardsAscendingBySelectedMetric);

        cardsInPackService.CardsInPack.CollectionChanged += CardsInPack.Update;
        cardsInPackService.CardsMissingFromPack.CollectionChanged += CardsMissingFromPack.Update;
        cardsInPackService.CardsUpcomingAfterPack.CollectionChanged += CardsUpcomingAfterPack.Update;
        cardsInPackService.CardsSeen.CollectionChanged += SeenCards.UpdateCardsSeen;
        cardsInPackService.PacksSeen.CollectionChanged += SeenCards.UpdatePacksSeen;
        cardsInPackService.Deck.CardStacks.CollectionChanged += UpdateDeck;
        cardsInPackService.ObservableForProperty(x => x.PreviousPick)
                          .Subscribe(x => UpdatePreviouslyPickedCardFromPack(x.Value));

        statisticsService.Statistics.CollectionChanged += UpdateStatistics;

        cryptService.Skeletons.CollectionChanged += UpdateSkeletons;

        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder, true)
                                                    .Subscribe(_ => ClearMetricViewModels());
        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder, true)
                                                    .Subscribe(_ => ClearStatisticsViewModels());
        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder, true)
                                                    .Subscribe(_ => ClearCardData());
        model.CardDataSourceBuilderCollectionService.ObservableForProperty(p => p.CurrentCardDataSourceBuilder)
                                                    .Subscribe(p => UpdateMetricViewModels(p.Value));

        model.ObservableForProperty(p => p.IsCardDataSourceBeingBuilt)
             .Subscribe(p => IsCardDataSourceBeingBuilt = p.Value);

        UpdateMetricViewModels(cardDataSourceBuilderCollectionService.CurrentCardDataSourceBuilder);

        InitializeStatistics();

        InitializeSkeletons();

        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(_ => CardsInPack.Sort());

        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(_ => SeenCards.Sort());

        this.ObservableForProperty(p => p.ShowSkeletonCounts)
            .Subscribe(v => UpdateSkeletonCountVisibility(v.Value));

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
        ShowSettingsDialog = new Interaction<SettingsDialogViewModel, bool>();

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

        ShowSettingsCommand = ReactiveCommand.Create(async () =>
        {
            var settingsViewModel = new SettingsDialogViewModel();
            var result = await ShowSettingsDialog.Handle(settingsViewModel);
            if (result)
            {
                await Configuration.Instance.Save();
            }
        });

        ClearCardsSeenAndDeckCommand = ReactiveCommand.Create(() =>
        {
            cardsInPackService.ClearPersistentState();
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

    internal CardListViewModel CardsInPack { get; }

    internal CardDataViewModel? PreviouslyPickedCardFromPack
    {
        get => previouslyPickedCardFromPack;
        private set => this.RaiseAndSetIfChanged(ref previouslyPickedCardFromPack, value);
    }

    internal CardListViewModel CardsMissingFromPack { get; }

    internal CardListViewModel CardsUpcomingAfterPack { get; }

    internal SeenCardsViewModel SeenCards { get; }

    internal ObservableCollection<CardStackViewModel> CardStacksInDeck { get; } = [];

    internal ObservableCollection<SkeletonViewModel> Skeletons { get; } = [];

    internal bool ShowSkeletonCounts
    { 
        get => showSkeletonCounts;
        set => this.RaiseAndSetIfChanged(ref showSkeletonCounts, value);
    }

    internal ICommand ShowLogCommand { get; init; }

    internal Interaction<CardDataSourceSettingsDialogViewModel, bool> ShowCardDataSourceSettingsDialog { get; }

    internal Interaction<SettingsDialogViewModel, bool> ShowSettingsDialog { get; }

    internal ICommand ShowCardDataSourceSettingsCommand { get; init; }

    internal ICommand ShowSettingsCommand { get; init; }

    internal ICommand ClearCardsSeenAndDeckCommand { get; init; }

    internal bool IsCardDataSourceBeingBuilt
    {
        get => isCardDataSourceBeingBuilt;
        private set => this.RaiseAndSetIfChanged(ref isCardDataSourceBeingBuilt, value);
    }

    private static void SortCardsAscending<T, TKey>(ObservableCollection<T> collection, Func<T, TKey> selector, IComparer<TKey>? comparer = null) where T : CardDataViewModel
    {
        List<T> sorted = [.. collection.OrderBy(selector, comparer ?? Comparer<TKey>.Default)];
        for (int i = 0; i < sorted.Count; i++)
        {
            collection.Move(collection.IndexOf(sorted[i]), i);
            sorted[i].Index = i;
        }
    }

    private static void SortCardsDescending<T, TKey>(ObservableCollection<T> collection, Func<T, TKey> selector, IComparer<TKey>? comparer = null) where T : CardDataViewModel
    {
        List<T> sorted = [.. collection.OrderByDescending(selector, comparer ?? Comparer<TKey>.Default)];
        for (int i = 0; i < sorted.Count; i++)
        {
            collection.Move(collection.IndexOf(sorted[i]), i);
            sorted[i].Index = i;
        }
    }

    private void UpdateDeck(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void ProcessNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is List<ICardDataService> cardStack)
                    {
                        var cardStackViewModel = new CardStackViewModel(_ => true, (c, i) => MakeCardDataViewModel(c, i), _ => {});
                        cardStackViewModel.AddCards(cardStack);
                        CardStacksInDeck.Add(cardStackViewModel);                        
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataService as a new item");
                    }
                }
            }
        }

        void ProcessOldItems(IList? oldItems)
        {
            if (oldItems != null)
            {
                foreach (var item in oldItems)
                {
                    if (item is List<ICardDataService> cardStack)
                    {
                        CardStacksInDeck.Remove(CardStacksInDeck.Where(deckStack => deckStack.Cards.Count == cardStack.Count && deckStack.Cards.Select(x => x.CardDataService).All(cardStack.Contains)));
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ICardDataService as a old item");
                    }
                }
            }
        }

        void ClearItems()
        {
            CardStacksInDeck.Clear();
        }

        DispatchObservableCollectionChanges(e, ClearItems, ProcessNewItems, ProcessOldItems);

        this.RaisePropertyChanged(nameof(CardStacksInDeck));
    }

    private void DispatchObservableCollectionChanges(System.Collections.Specialized.NotifyCollectionChangedEventArgs e, Action clearItems, Action<IList?> processNewItems, Action<IList?> processOldItems)
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
        CardDataViewModel? vm = null;
        if (cardDataService != null)
        {
            vm = MakeCardDataViewModel(cardDataService, 0, true);
        }
        PreviouslyPickedCardFromPack = vm;
        
        this.RaisePropertyChanged(nameof(PreviouslyPickedCardFromPack));
    }

    private void InitializeStatistics()
    {
        foreach (var statisticService in statisticsService.Statistics)
        {
            Statistics.Add(new CardDataSourceStatisticViewModel(statisticService));
        }
    }

    private void InitializeSkeletons()
    {
        foreach (var skeletonService in cryptService.Skeletons)
        {
            var skeleton = new SkeletonViewModel(skeletonService);
            skeleton.UpdateCardMetricVisibility(c => UpdateCardMetricVisibility(c));
            Skeletons.Add(skeleton);
        }
    }

    private void UpdateStatistics(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void ProcessNewItems(IList? newItems)
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

        void ProcessOldItems(IList? oldItems)
        {
            if (oldItems != null)
            {
                foreach (var item in oldItems)
                {
                    if (item is ICardDataSourceStatisticService)
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

        void ClearItems()
        {
            Statistics.Clear();
        }

        DispatchObservableCollectionChanges(e, ClearItems, ProcessNewItems, ProcessOldItems);
    }

    private void UpdateSkeletons(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void ProcessNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ISkeletonService skeletonService)
                    {
                        var skeleton = new SkeletonViewModel(skeletonService);
                        skeleton.UpdateCardMetricVisibility(c => UpdateCardMetricVisibility(c));
                        Skeletons.Add(skeleton);
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ISkeletonService as a new item");
                    }
                }
            }
        }

        void ProcessOldItems(IList? oldItems)
        {
            if (oldItems != null)
            {
                foreach (var item in oldItems)
                {
                    if (item is ISkeletonService)
                    {
                        Skeletons.Remove(Skeletons.Where(x => x.SkeletonService == item));
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a ISkeletonService as a old item");
                    }
                }
            }
        }

        void ClearItems()
        {
            Skeletons.Clear();
        }

        DispatchObservableCollectionChanges(e, ClearItems, ProcessNewItems, ProcessOldItems);
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
        SeenCards.Clear();
        CardStacksInDeck.Clear();
        PreviouslyPickedCardFromPack = null;
        cryptService.Clear();
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

            var comparisonResult = internalComparer.Compare(x.Metrics.Metrics[metricIndex].Metric, y.Metrics.Metrics[metricIndex].Metric);
            if (comparisonResult == 0)
            {
                return x.CardDataService.ArenaCardId.CompareTo(y.CardDataService.ArenaCardId);
            }

            return comparisonResult;
        }

        private readonly IComparer<ICardMetric> internalComparer;
        private readonly int metricIndex;
    };

    private void SortCardsAscendingBySelectedMetric(ObservableCollection<CardDataViewModel> collection)
    {
        if (sortByMetricDescriptionViewModel == null)
        {
            return;
        }

        IComparer<ICardMetric> internalComparer = sortByMetricDescriptionViewModel.Comparer;
        int metricIndex = metricDescriptionViewModels.IndexOf(sortByMetricDescriptionViewModel);

        CardDataViewModelComparer comparer = new CardDataViewModelComparer(internalComparer, metricIndex);
        SortCardsAscending(collection, x => x, comparer);
    }
    
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

        CardsInPack.UpdateMetricVisibility(metricIndex, visible);

        if(PreviouslyPickedCardFromPack != null)
        {
            PreviouslyPickedCardFromPack.Metrics.Metrics[metricIndex].Visible = visible;
        }

        CardsMissingFromPack.UpdateMetricVisibility(metricIndex, visible);

        CardsUpcomingAfterPack.UpdateMetricVisibility(metricIndex, visible);

        SeenCards.UpdateMetricVisibility(metricIndex, visible);

        foreach (var stack in CardStacksInDeck)
        {
            stack.UpdateMetricVisibility(metricIndex, visible);
        }

        foreach(var skeleton in Skeletons)
        {
            skeleton.UpdateCardMetricVisibility(c => c.Metrics.Metrics[metricIndex].Visible = visible);
        }
    }

    private CardDataViewModel UpdateCardMetricVisibility(CardDataViewModel cardVm)
    {
        for(int i = 0; i < metricDescriptionViewModels.Count; ++i)
        {
            cardVm.Metrics.Metrics[i].Visible = metricDescriptionViewModels[i].Visible;
        }

        return cardVm;
    }

    private CardDataViewModel MakeCardDataViewModel(ICardDataService cardDataService, int index, bool highlight = false)
    {
        return UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, index, highlight));
    }

    private void ClearCardMetricVisibility(CardDataViewModel cardVm)
    {
        for (int i = 0; i < metricDescriptionViewModels.Count; ++i)
        {
            cardVm.Metrics.Metrics[i].Visible = false;
        }
    }

    private void UpdateSkeletonCountVisibility(bool visible)
    {
        foreach (var skeleton in Skeletons)
        {
            skeleton.UpdateCountVisibility(visible);
        }
    }

    private readonly ICardDataSourceBuilderCollectionService cardDataSourceBuilderCollectionService;
    private readonly ICardsInPackService cardsInPackService;
    private readonly ICardDataSourceStatisticsService statisticsService;
    private readonly ICryptService cryptService;

    private readonly LogWindow logWindow;

    private ObservableCollection<CardDataMetricDescriptionViewModel> metricDescriptionViewModels;
    private CardDataMetricDescriptionViewModel? sortByMetricDescriptionViewModel;

    private ObservableCollection<CardDataSourceStatisticViewModel> statisticsViewModels;

    private CardDataViewModel? previouslyPickedCardFromPack = null;

    private bool isCardDataSourceBeingBuilt = false;

    private bool showSkeletonCounts = true;
}
