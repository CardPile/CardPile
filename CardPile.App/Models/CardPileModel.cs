using Avalonia.Threading;
using CardPile.App.Services;
using CardPile.CardData;
using CardPile.CardData.Parameters;
using CardPile.CardData.Settings;
using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CardPile.App.Models;

internal class CardPileModel : ReactiveObject
{
    internal static async Task Init(Action<string> progressCallback, CancellationToken cancellationToken)
    {
        progressCallback("Loading Arena data...");
        await Task.Run(CardInfo.Arena.Init, cancellationToken);
    }

    internal CardPileModel()
    {
        logModel = new LogModel();

        cardDataSourceBuilderCollectionModel = new CardDataSourceBuilderCollectionModel();
        cardDataSourceBuilderCollectionModel.ObservableForProperty(x => x.CurrentCardDataSourceBuilder)
                                            .Subscribe(x => SwitchCardDataSourceBuilder(x.Value));

        cardDataSource = cardDataSourceBuilderCollectionModel.CurrentCardDataSourceBuilder.BuildDataSource();
        SubscribeToAllBuilderSourceParameters(cardDataSourceBuilderCollectionModel.CurrentCardDataSourceBuilder);
        SubscribeToAllBuilderSourceSettings(cardDataSourceBuilderCollectionModel.CurrentCardDataSourceBuilder);

        watcherModel = new WatcherModel();
        draftModel = new DraftModel(watcherModel, cardDataSource);
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
                       .Subscribe(x => BuildCardDataSource(builder));
            }
            if(parameter.Type == ParameterType.Date)
            {
                var date = parameter as ICardDataSourceParameterDateService;
                date.ObservableForProperty(x => x.Value)
                    .Subscribe(x => BuildCardDataSource(builder));
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
                    .Subscribe (x => BuildCardDataSource(builder));
            }
        }
    }

    private void BuildCardDataSource(ICardDataSourceBuilderService builder)
    {
        buildCardDataCancellationTokenSource?.Cancel();
        buildCardDataCancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => builder.BuildDataSourceAsync(buildCardDataCancellationTokenSource.Token)).ContinueWith(x => Dispatcher.UIThread.Post(() => draftModel.SetCardDataSource(x.Result)), TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    private CardDataSourceBuilderCollectionModel cardDataSourceBuilderCollectionModel;
    private WatcherModel watcherModel;
    private ICardDataSource cardDataSource;
    private DraftModel draftModel;
    private LogModel logModel;

    private CancellationTokenSource? buildCardDataCancellationTokenSource = null;
}
