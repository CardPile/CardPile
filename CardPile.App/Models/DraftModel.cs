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
        draftState = new DraftState();

        cardsInCurrentPack = [];
        cardsMissingInCurrentPack = [];
        cardsUpcomingAfterCurrentPack = [];
        cardsSeen = [];
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

    public ObservableCollection<ICardDataService> CardsSeen
    {
        get => cardsSeen;
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
        UpdateCardDataAfterChoice(cardDataSource);
        UpdateCardDataAfterPick(cardDataSource);
    }

    private void DraftEnterHandler(object? sender, DraftEnterEvent e)
    {
        draftState.ProcessEvent(e);

        cardsInCurrentPack.Clear();
        cardsMissingInCurrentPack.Clear();
        cardsUpcomingAfterCurrentPack.Clear();
        cardsSeen.Clear();
        cardsInDeck.Clear();

        PreviousPick = null;
    }

    private void DraftChoiceHandler(object? sender, DraftChoiceEvent e)
    {
        if(draftState.TrySetDraftId(e.DraftId))
        {
            DeserializeDraftState(draftState);
        }

        draftState.ProcessEvent(e);

        UpdateCardDataAfterChoice(cardDataSource);

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
        draftState.TrySetDraftId(e.DraftId);

        draftState.ProcessEvent(e);

        SerializeDraftState(draftState);

        if(e.PackNumber == draftState.LastPack && e.PickNumber == draftState.LastPick)
        {
            cardsInCurrentPack.Clear();
            cardsMissingInCurrentPack.Clear();

            PreviousPick = null;
        }

        UpdateCardDataAfterPick(cardDataSource);

        var deck = draftState.GetCurrentDeck();
        logger.Info($"Current deck [{string.Join(",", deck)}]");
    }

    private void DraftLeaveHandler(object? sender, DraftLeaveEvent e)
    {
        cardsInCurrentPack.Clear();
        cardsMissingInCurrentPack.Clear();
        cardsUpcomingAfterCurrentPack.Clear();
        cardsSeen.Clear();
        cardsInDeck.Clear();

        PreviousPick = null;

        draftState.ProcessEvent(e);
    }

    private void UpdateCardDataAfterChoice(ICardDataSource cardDataSource)
    {
        cardsInCurrentPack.Clear();
        cardsMissingInCurrentPack.Clear();
        cardsUpcomingAfterCurrentPack.Clear();

        PreviousPick = null;

        foreach (var cardInPack in draftState.CurrentPack)
        {
            var cardInPackData = cardDataSource.GetDataForCard(cardInPack, draftState);
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
            var missingCardData = cardDataSource.GetDataForCard(missingCard, draftState);
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
            var upcomingCardData = cardDataSource.GetDataForCard(upcomingCard, draftState);
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
            var previouslyPickedCardData = cardDataSource.GetDataForCard(previouslyPickedCard.Value, draftState);
            if(previouslyPickedCardData != null)
            {
                PreviousPick = new CardDataModel(previouslyPickedCardData);
            }
        }
    }

    private void UpdateCardDataAfterPick(ICardDataSource cardDataSource)
    {
        cardsInDeck.Clear();
        cardsSeen.Clear();

        foreach (var cardInDeck in draftState.GetCurrentDeck())
        {
            var cardInDeckData = cardDataSource.GetDataForCard(cardInDeck, draftState);
            if (cardInDeckData != null)
            {
                cardsInDeck.Add(new CardDataModel(cardInDeckData));
            }
        }

        foreach (var seenCard in draftState.GetSeenCards())
        {
            var seendCardData = cardDataSource.GetDataForCard(seenCard, draftState);
            if (seendCardData != null)
            {
                cardsSeen.Add(new CardDataModel(seendCardData));
            }
            else
            {
                logger.Info(string.Format($"Could not find card data for seen card with MTGA id {seenCard}"));
            }
        }
    }

    private static void SerializeDraftState(DraftState state)
    {
        if(state.DraftId == Guid.Empty)
        {
            return;
        }

        string executableDirectory = Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".";
        string draftDirectory = Path.Combine(executableDirectory, "Drafts");
        string draftFilePath = Path.Combine(draftDirectory, $"{state.DraftId}.json");

        if(!Directory.Exists(draftDirectory))
        {
            Directory.CreateDirectory(draftDirectory);
        }

        JsonSerializer serializer = new JsonSerializer();

        using StreamWriter sw = new(draftFilePath);
        using JsonTextWriter jsonWriter = new(sw);

        serializer.Serialize(jsonWriter, state);
    }

    private static void DeserializeDraftState(DraftState state)
    {
        if (state.DraftId == Guid.Empty)
        {
            return;
        }

        string executableDirectory = Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".";
        string draftDirectory = Path.Combine(executableDirectory, "Drafts");
        string draftFilePath = Path.Combine(draftDirectory, $"{state.DraftId}.json");

        if (!Directory.Exists(draftDirectory))
        {
            return;
        }

        if (!File.Exists(draftFilePath))
        {
            return;
        }

        JsonSerializer serializer = new();

        using StreamReader sr = new(draftFilePath);
        using JsonTextReader jsonReader = new(sr);

        serializer.Populate(jsonReader, state);
    }

    private DraftState draftState;

    private readonly ObservableCollection<ICardDataService> cardsInCurrentPack;
    private readonly ObservableCollection<ICardDataService> cardsMissingInCurrentPack;
    private readonly ObservableCollection<ICardDataService> cardsUpcomingAfterCurrentPack;
    private readonly ObservableCollection<ICardDataService> cardsSeen;
    private readonly ObservableCollection<ICardDataService> cardsInDeck;
    private ICardDataService? previousPick;

    private readonly WatcherModel logModel;
    private ICardDataSource cardDataSource;

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
