using Avalonia.Threading;
using CardPile.App.Services;
using CardPile.CardData.Parameters;
using CardPile.CardData.Settings;
using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;
using CardPile.CardInfo;

namespace CardPile.App.Models;

internal class CardPileModel : ReactiveObject
{
    internal static async Task Init(Action<string> progressCallback, CancellationToken cancellationToken)
    {
        progressCallback("Loading Arena data...");
        await Task.Run(Arena.Init, cancellationToken);
        
        progressCallback("Cleaning old card images...");
        await Task.Run(Scryfall.ClearOldImages, cancellationToken);

        progressCallback("Cleaning old drafts...");
        await Task.Run(DraftModel.ClearOldDrafts, cancellationToken);

        progressCallback("Initializing images...");
        await Task.Run(Scryfall.Init, cancellationToken);
        
        progressCallback("Initializing card data sources...");
        await Task.Run(CardDataSourceBuilderCollectionModel.Init, cancellationToken);
    }

    internal CardPileModel()
    {
        logModel = new LogModel();

        cardDataSourceBuilderCollectionModel = new CardDataSourceBuilderCollectionModel();
        cardDataSourceBuilderCollectionModel.ObservableForProperty(x => x.CurrentCardDataSourceBuilder)
                                            .Subscribe(x => SwitchCardDataSourceBuilder(x.Value));

        var cardDataSource = cardDataSourceBuilderCollectionModel.CurrentCardDataSourceBuilder.BuildDataSource();
        SubscribeToAllBuilderSourceParameters(cardDataSourceBuilderCollectionModel.CurrentCardDataSourceBuilder);
        SubscribeToAllBuilderSourceSettings(cardDataSourceBuilderCollectionModel.CurrentCardDataSourceBuilder);
        
        draftModel = new DraftModel(new WatcherModel(), cardDataSource);
        statisticsModel = new CardDataSourceStatisticsModel(cardDataSource);
    }

    public ICardDataSourceBuilderCollectionService CardDataSourceBuilderCollectionService
    {
        get => cardDataSourceBuilderCollectionModel;
    }

    public ILogService LogService
    {
        get => logModel;
    }

    public ICardsInPackService CurrentCardsInPackService
    {
        get => draftModel;
    }

    public ICardDataSourceStatisticsService StatisticsService
    {
        get => statisticsModel;
    }

    public bool IsCardDataSourceBeingBuilt
    {
        get => isCardDataSourceBeingBuilt;
        private set => this.RaiseAndSetIfChanged(ref isCardDataSourceBeingBuilt, value);
    }

    private void SwitchCardDataSourceBuilder(ICardDataSourceBuilderService builder)
    {
        SubscribeToAllBuilderSourceParameters(builder);
        BuildCardDataSource(builder);
        SubscribeToAllBuilderSourceSettings(builder);
    }

    private void SubscribeToAllBuilderSourceParameters(ICardDataSourceBuilderService builder)
    {
        foreach (var parameter in builder.Parameters)
        {
            if (parameter.Type == ParameterType.Options)
            {
                var options = parameter as ICardDataSourceParameterOptionsService;
                options.ObservableForProperty(x => x.Value)
                       .Subscribe(_ => BuildCardDataSource(builder));
            }
            if(parameter.Type == ParameterType.Date)
            {
                var date = parameter as ICardDataSourceParameterDateService;
                date.ObservableForProperty(x => x.Value)
                    .Subscribe(_ => BuildCardDataSource(builder));
            }
        }
    }

    private void SubscribeToAllBuilderSourceSettings(ICardDataSourceBuilderService builder)
    {
        foreach(var setting in builder.Settings)
        {
            if(setting.Type == SettingType.Path)
            {
                var path = setting as ICardDataSourceSettingPathService;
                path.ObservableForProperty(x => x.Value)
                    .Subscribe (_ => BuildCardDataSource(builder));
            }
        }
    }

    private void BuildCardDataSource(ICardDataSourceBuilderService builder)
    {
        buildCardDataSourceCancellationToken?.Cancel();
        IsCardDataSourceBeingBuilt = false;

        buildCardDataSourceCancellationToken = new CancellationTokenSource();
        Task.Run(() => builder.BuildDataSourceAsync(buildCardDataSourceCancellationToken.Token))
                              .ContinueWith(x => Dispatcher.UIThread.Post(() => { draftModel.SetCardDataSource(x.Result); statisticsModel.SetCardDataSource(x.Result); }), TaskContinuationOptions.OnlyOnRanToCompletion)
                              .ContinueWith(_ => Dispatcher.UIThread.Post(() => IsCardDataSourceBeingBuilt = false));

        IsCardDataSourceBeingBuilt = true;
    }

    private readonly CardDataSourceBuilderCollectionModel cardDataSourceBuilderCollectionModel;
    private readonly DraftModel draftModel;
    private readonly CardDataSourceStatisticsModel statisticsModel;
    private readonly LogModel logModel;

    private CancellationTokenSource? buildCardDataSourceCancellationToken;
    private bool isCardDataSourceBeingBuilt;
}
