using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Data.Converters;
using CardPile.Application.Models;
using CardPile.Application.Services;
using CardPile.Application.Views;
using CardPile.CardData;
using CardPile.CardData.Metrics;
using CardPile.CardData.Settings;
using DynamicData;
using ReactiveUI;

namespace CardPile.Application.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public static FuncValueConverter<ICollection<CardDataViewModel>, int> CardCollectionToStackHeightConverter { get; } = new FuncValueConverter<ICollection<CardDataViewModel>, int>(collection =>
    {
        if (collection == null || collection.Count == 0)
        {
            return 0;
        }

        return (collection.Count - 1) * CardDataModel.CARD_HEADER_SIZE + CardDataModel.CARD_IMAGE_HEIGHT;
    });

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

        cardsInPackService.CardsInPack.CollectionChanged += UpdateCardsInPack;
        cardsInPackService.CardsMissingFromPack.CollectionChanged += UpdateCardsMissingFromPack;
        cardsInPackService.CardsUpcomingAfterPack.CollectionChanged += UpdateCardsUpcomingAfterPack;
        cardsInPackService.CardsSeen.CollectionChanged += UpdateCardsSeen;
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
            .Subscribe(_ => SortCardsDescendingBySelectedMetric(CardsInPack.Cards));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(_ => SortCardsDescendingBySelectedMetric(WhiteCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(_ => SortCardsDescendingBySelectedMetric(BlueCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(_ => SortCardsDescendingBySelectedMetric(BlackCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(_ => SortCardsDescendingBySelectedMetric(RedCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(_ => SortCardsDescendingBySelectedMetric(GreenCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(_ => SortCardsDescendingBySelectedMetric(MulticolorCardsSeen));
        this.ObservableForProperty(p => p.SortByMetricDescription)
            .Subscribe(_ => SortCardsDescendingBySelectedMetric(ColorlessCardsSeen));

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

    internal CardListViewModel CardsInPack { get; } = [];

    internal CardDataViewModel? PreviouslyPickedCardFromPack
    {
        get => previouslyPickedCardFromPack;
        private set => this.RaiseAndSetIfChanged(ref previouslyPickedCardFromPack, value);
    }

    internal CardListViewModel CardsMissingFromPack { get; } = [];

    internal CardListViewModel CardsUpcomingAfterPack { get; } = [];

    internal ObservableCollection<CardDataViewModel> WhiteCardsSeen { get; } = [];

    internal ObservableCollection<CardDataViewModel> BlueCardsSeen { get; } = [];

    internal ObservableCollection<CardDataViewModel> BlackCardsSeen { get; } = [];

    internal ObservableCollection<CardDataViewModel> RedCardsSeen { get; } = [];

    internal ObservableCollection<CardDataViewModel> GreenCardsSeen { get; } = [];

    internal ObservableCollection<CardDataViewModel> MulticolorCardsSeen { get; } = [];

    internal ObservableCollection<CardDataViewModel> ColorlessCardsSeen { get; } = [];

    internal ObservableCollection<List<CardDataViewModel>> CardStacksInDeck { get; } = [];

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

    private static void SortCards<T, TKey>(ObservableCollection<T> collection, Func<T, TKey> selector, IComparer<TKey>? comparer = null) where T : CardDataViewModel
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

    private void UpdateCardsInPack(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void ProcessNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        CardsInPack.Cards.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, CardsInPack.Cards.Count)));
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
                    if (item is ICardDataService)
                    {
                        CardsInPack.Cards.Remove(CardsInPack.Cards.Where(x => x.CardDataService == item));
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
            CardsInPack.Cards.Clear();
        }

        DispatchObservableCollectionChanges(e, ClearItems, ProcessNewItems, ProcessOldItems);
        SortCardsDescendingBySelectedMetric(CardsInPack.Cards);
    }

    private void UpdateCardsMissingFromPack(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void ProcessNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        CardsMissingFromPack.Cards.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, CardsMissingFromPack.Cards.Count)));
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
                    if (item is ICardDataService)
                    {
                        CardsMissingFromPack.Cards.Remove(CardsMissingFromPack.Cards.Where(x => x.CardDataService == item));
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
            CardsMissingFromPack.Cards.Clear();
        }

        DispatchObservableCollectionChanges(e, ClearItems, ProcessNewItems, ProcessOldItems);
        
        SortCardsBySelectedMetric(CardsMissingFromPack.Cards);
        
        this.RaisePropertyChanged(nameof(CardsMissingFromPack));
    }

    private void UpdateCardsUpcomingAfterPack(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void ProcessNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        CardsUpcomingAfterPack.Cards.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, CardsUpcomingAfterPack.Cards.Count)));
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
                    if (item is ICardDataService)
                    {
                        CardsUpcomingAfterPack.Cards.Remove(CardsUpcomingAfterPack.Cards.Where(x => x.CardDataService == item));
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
            CardsUpcomingAfterPack.Cards.Clear();
        }

        DispatchObservableCollectionChanges(e, ClearItems, ProcessNewItems, ProcessOldItems);
        
        SortCardsDescendingBySelectedMetric(CardsMissingFromPack.Cards);
        
        this.RaisePropertyChanged(nameof(CardsUpcomingAfterPack));
    }

    private void UpdateCardsSeen(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        void ProcessNewItems(IList? newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is ICardDataService cardDataService)
                    {
                        if (cardDataService.Colors.Count() == 0)
                        {
                            ColorlessCardsSeen.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, ColorlessCardsSeen.Count)));
                        }
                        else if (cardDataService.Colors.Count() == 1)
                        {
                            if(cardDataService.Colors == Color.White)
                            {
                                WhiteCardsSeen.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, WhiteCardsSeen.Count)));
                            }
                            else if(cardDataService.Colors == Color.Blue)
                            {
                                BlueCardsSeen.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, BlueCardsSeen.Count)));
                            }
                            else if (cardDataService.Colors == Color.Black)
                            {
                                BlackCardsSeen.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, BlackCardsSeen.Count)));
                            }
                            else if (cardDataService.Colors == Color.Red)
                            {
                                RedCardsSeen.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, RedCardsSeen.Count)));
                            }
                            else if (cardDataService.Colors == Color.Green)
                            {
                                GreenCardsSeen.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, GreenCardsSeen.Count)));
                            }
                        }
                        else
                        {
                            MulticolorCardsSeen.Add(UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, MulticolorCardsSeen.Count)));
                        }
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
                    if (item is ICardDataService cardDataService)
                    {
                        if (cardDataService.Colors.Count() == 0)
                        {
                            ColorlessCardsSeen.Remove(ColorlessCardsSeen.Where(x => x.CardDataService == item));
                        }
                        else if (cardDataService.Colors.Count() == 1)
                        {
                            if (cardDataService.Colors == Color.White)
                            {
                                WhiteCardsSeen.Remove(WhiteCardsSeen.Where(x => x.CardDataService == item));
                            }
                            else if (cardDataService.Colors == Color.Blue)
                            {
                                BlueCardsSeen.Remove(BlueCardsSeen.Where(x => x.CardDataService == item));
                            }
                            else if (cardDataService.Colors == Color.Black)
                            {
                                BlackCardsSeen.Remove(BlackCardsSeen.Where(x => x.CardDataService == item));
                            }
                            else if (cardDataService.Colors == Color.Red)
                            {
                                RedCardsSeen.Remove(RedCardsSeen.Where(x => x.CardDataService == item));
                            }
                            else if (cardDataService.Colors == Color.Green)
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

        void ClearItems()
        {
            WhiteCardsSeen.Clear();
            BlueCardsSeen.Clear();
            BlackCardsSeen.Clear();
            RedCardsSeen.Clear();
            GreenCardsSeen.Clear();
            MulticolorCardsSeen.Clear();
            ColorlessCardsSeen.Clear();
        }

        DispatchObservableCollectionChanges(e, ClearItems, ProcessNewItems, ProcessOldItems);

        SortCardsDescendingBySelectedMetric(WhiteCardsSeen);
        SortCardsDescendingBySelectedMetric(BlueCardsSeen);
        SortCardsDescendingBySelectedMetric(BlackCardsSeen);
        SortCardsDescendingBySelectedMetric(RedCardsSeen);
        SortCardsDescendingBySelectedMetric(GreenCardsSeen);
        SortCardsDescendingBySelectedMetric(MulticolorCardsSeen);
        SortCardsDescendingBySelectedMetric(ColorlessCardsSeen);

        this.RaisePropertyChanged(nameof(WhiteCardsSeen));
        this.RaisePropertyChanged(nameof(BlueCardsSeen));
        this.RaisePropertyChanged(nameof(BlackCardsSeen));
        this.RaisePropertyChanged(nameof(RedCardsSeen));
        this.RaisePropertyChanged(nameof(GreenCardsSeen));
        this.RaisePropertyChanged(nameof(MulticolorCardsSeen));
        this.RaisePropertyChanged(nameof(ColorlessCardsSeen));
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
                        var cardViewModelStack = cardStack.Select((c, i) => UpdateCardMetricVisibility(new CardDataViewModel(c, i))).ToList();;
                        CardStacksInDeck.Add(cardViewModelStack);
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
                        CardStacksInDeck.Remove(CardStacksInDeck.Where(deckStack => deckStack.Count == cardStack.Count && deckStack.Select(x => x.CardDataService).All(cardStack.Contains)));
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
            vm = UpdateCardMetricVisibility(new CardDataViewModel(cardDataService, 0, true));
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
        CardsInPack.Cards.Clear();
        CardsMissingFromPack.Cards.Clear();
        CardsUpcomingAfterPack.Cards.Clear();
        WhiteCardsSeen.Clear();
        BlueCardsSeen.Clear();
        BlackCardsSeen.Clear();
        RedCardsSeen.Clear();
        GreenCardsSeen.Clear();
        MulticolorCardsSeen.Clear();
        ColorlessCardsSeen.Clear();
        CardStacksInDeck.Clear();
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

    private void SortCardsBySelectedMetric(ObservableCollection<CardDataViewModel> collection)
    {
        if (sortByMetricDescriptionViewModel == null)
        {
            return;
        }

        IComparer<ICardMetric> internalComparer = sortByMetricDescriptionViewModel.Comparer;
        int metricIndex = metricDescriptionViewModels.IndexOf(sortByMetricDescriptionViewModel);

        CardDataViewModelComparer comparer = new CardDataViewModelComparer(internalComparer, metricIndex);
        SortCards(collection, x => x, comparer);
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
        foreach (var card in CardsInPack.Cards)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        if(PreviouslyPickedCardFromPack != null)
        {
            PreviouslyPickedCardFromPack.Metrics.Metrics[metricIndex].Visible = visible;
        }


        foreach (var card in CardsMissingFromPack.Cards)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        foreach (var card in CardsUpcomingAfterPack.Cards)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        foreach (var card in WhiteCardsSeen)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        foreach (var card in BlueCardsSeen)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        foreach (var card in BlackCardsSeen)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        foreach (var card in RedCardsSeen)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        foreach (var card in GreenCardsSeen)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        foreach (var card in MulticolorCardsSeen)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        foreach (var card in ColorlessCardsSeen)
        {
            card.Metrics.Metrics[metricIndex].Visible = visible;
        }

        foreach (var stack in CardStacksInDeck)
        {
            foreach(var card in stack)
            {
                card.Metrics.Metrics[metricIndex].Visible = visible;
            }
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
