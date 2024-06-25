using CardPile.App.Services;
using CardPile.Draft;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.ObjectModel;
using NLog;
using System;
using System.IO;
using Newtonsoft.Json;

namespace CardPile.App.Models;

internal class DraftModel : ReactiveObject, ICardsInPackService
{
    public DraftModel(WatcherModel logModel, ICardDataSource cardDataSource)
    {
        CardInfo.Arena.Init();
        CardInfo.Scryfall.Init();
        CardInfo.SeventeenLands.Init();

        draftState = new DraftState();

        cardsInCurrentPack = [];
        cardsMissingInCurrentPack = [];
        cardsUpcomingAfterCurrentPack = [];
        cardsInDeck = [];
        
        PreviousPick = null;

        this.logModel = logModel;
        this.logModel.DraftEnterEvent += DraftEnterHandler;
        this.logModel.DraftChoiceEvent += DraftChoiceHandler;
        this.logModel.DraftPickEvent += DraftPickHandler;
        this.logModel.DraftLeaveEvent += DraftLeaveHandler;

        this.cardDataSource = cardDataSource;
    }

    public ObservableCollection<ICardDataService> CardsInPack
    {
        get => cardsInCurrentPack;
    }

    public ObservableCollection<ICardDataService> CardsMissingFromPack
    {
        get => cardsMissingInCurrentPack;
    }

    public ObservableCollection<ICardDataService> CardsUpcomingAfterPack
    {
        get => cardsUpcomingAfterCurrentPack;
    }

    public ICardDataService? PreviousPick
    {
        get => previousPick;
        set => this.RaiseAndSetIfChanged(ref previousPick, value);
    }

    internal void SetCardDataSource(ICardDataSource newCardDataSource)
    {
        logger.Info($"Setting new card data source {newCardDataSource.Name}");
        cardDataSource = newCardDataSource;
        UpdateCurrentPackWithNewData(cardDataSource);
        UpdateDeckWithNewData(cardDataSource);
    }

    private void DraftEnterHandler(object? sender, DraftEnterEvent e)
    {
        draftState.ProcessEvent(e);

        cardsInCurrentPack.Clear();
        cardsMissingInCurrentPack.Clear();
        cardsUpcomingAfterCurrentPack.Clear();
        cardsInDeck.Clear();

        PreviousPick = null;
    }

    private void DraftChoiceHandler(object? sender, DraftChoiceEvent e)
    {
        draftState.ProcessEvent(e);

        UpdateCurrentPackWithNewData(cardDataSource);

        var deck = draftState.GetCurrentDeck();
        logger.Info($"Current deck [{string.Join(",", deck)}]");

        var missing = draftState.GetCurrentMissingCards();
        if (missing.Count > 0)
        {
            logger.Info($"Missing cards [{string.Join(",", missing)}]");
        }

        var upcoming = draftState.GetCurrentUpcomingCards();
        if (upcoming.Count > 0)
        {
            logger.Info($"Upcoming cards [{string.Join(",", upcoming)}]");
        }
    }

    private void DraftPickHandler(object? sender, DraftPickEvent e)
    {
        draftState.ProcessEvent(e);

        SerializeDraftState();

        if(e.PackNumber == draftState.LastPack && e.PickNumber == draftState.LastPick)
        {
            cardsInCurrentPack.Clear();
            cardsMissingInCurrentPack.Clear();

            PreviousPick = null;
        }

        UpdateDeckWithNewData(cardDataSource);

        var deck = draftState.GetCurrentDeck();
        logger.Info($"Current deck [{string.Join(",", deck)}]");
    }

    private void DraftLeaveHandler(object? sender, DraftLeaveEvent e)
    {
        cardsInCurrentPack.Clear();
        cardsMissingInCurrentPack.Clear();
        cardsUpcomingAfterCurrentPack.Clear();
        cardsInDeck.Clear();

        PreviousPick = null;

        draftState.ProcessEvent(e);
    }

    private void UpdateCurrentPackWithNewData(ICardDataSource cardDataSource)
    {
        cardsInCurrentPack.Clear();
        cardsMissingInCurrentPack.Clear();
        cardsUpcomingAfterCurrentPack.Clear();

        PreviousPick = null;

        foreach (var cardInPack in draftState.CurrentPack)
        {
            var cardInPackData = cardDataSource.GetDataForCard(cardInPack);
            if (cardInPackData != null)
            {
                cardsInCurrentPack.Add(new CardDataModel(cardInPackData));
            }
            else
            {
                logger.Info(string.Format($"Could not find card data for in pack card with MTGA id {cardInPack}"));
            }
        }

        foreach (var missingCard in draftState.GetCurrentMissingCards())
        {
            var missingCardData = cardDataSource.GetDataForCard(missingCard);
            if (missingCardData != null)
            {
                cardsMissingInCurrentPack.Add(new CardDataModel(missingCardData));
            }
            else
            {
                logger.Info(string.Format($"Could not find card data for missing card with MTGA id {missingCard}"));
            }
        }

        foreach (var upcomingCard in draftState.GetCurrentUpcomingCards())
        {
            var upcomingCardData = cardDataSource.GetDataForCard(upcomingCard);
            if (upcomingCardData != null)
            {
                cardsUpcomingAfterCurrentPack.Add(new CardDataModel(upcomingCardData));
            }
            else
            {
                logger.Info(string.Format($"Could not find card data for upcoming card with MTGA id {upcomingCard}"));
            }
        }

        var previouslyPickedCard = draftState.GetCurrentPackPreviousPick();
        if (previouslyPickedCard != null)
        {
            var previouslyPickedCardData = cardDataSource.GetDataForCard(previouslyPickedCard.Value);
            if(previouslyPickedCardData != null)
            {
                PreviousPick = new CardDataModel(previouslyPickedCardData);
            }
        }
    }

    private void UpdateDeckWithNewData(ICardDataSource cardDataSource)
    {
        cardsInDeck.Clear();

        foreach (var cardInDeck in draftState.GetCurrentDeck())
        {
            var cardInDeckData = cardDataSource.GetDataForCard(cardInDeck);
            if (cardInDeckData != null)
            {
                cardsInDeck.Add(new CardDataModel(cardInDeckData));
            }
        }
    }

    private void SerializeDraftState()
    {
        if(draftState.DraftId == Guid.Empty)
        {
            return;
        }

        string executableDirectory = Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".";
        string draftDirectory = Path.Combine(executableDirectory, "Drafts");
        string draftFilePath = Path.Combine(draftDirectory, $"{draftState.DraftId}.json");

        if(!Directory.Exists(draftDirectory))
        {
            Directory.CreateDirectory(draftDirectory);
        }

        JsonSerializer serializer = new JsonSerializer();

        using StreamWriter sw = new StreamWriter(draftFilePath);
        using JsonWriter jsonWriter = new JsonTextWriter(sw);

        serializer.Serialize(jsonWriter, draftState);
    }

    private DraftState draftState;

    private ObservableCollection<ICardDataService> cardsInCurrentPack;
    private ObservableCollection<ICardDataService> cardsMissingInCurrentPack;
    private ObservableCollection<ICardDataService> cardsUpcomingAfterCurrentPack;
    private ObservableCollection<ICardDataService> cardsInDeck;
    private ICardDataService? previousPick;

    private WatcherModel logModel;
    private ICardDataSource cardDataSource;

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
