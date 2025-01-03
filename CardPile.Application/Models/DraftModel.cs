﻿using CardPile.Application.Services;
using CardPile.Draft;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.ObjectModel;
using NLog;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CardPile.Deck;

namespace CardPile.Application.Models;

internal class DraftModel : ReactiveObject, ICardsInPackService
{
    public DraftModel(WatcherModel logModel, ICardDataSource cardDataSource)
    {
        draftState = new DraftState();

        cardsInCurrentPack = [];
        cardsMissingInCurrentPack = [];
        cardsUpcomingAfterCurrentPack = [];
        cardsSeen = [];
        
        deck = new DeckModel();
        
        PreviousPick = null;

        this.logModel = logModel;
        this.logModel.DraftEnterEvent += DraftEnterHandler;
        this.logModel.DraftChoiceEvent += DraftChoiceHandler;
        this.logModel.DraftPickEvent += DraftPickHandler;
        this.logModel.DraftLeaveEvent += DraftLeaveHandler;

        SetCardDataSource(cardDataSource);
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

    public IDeckService Deck
    {
        get => deck;
        private set => this.RaiseAndSetIfChanged(ref deck, value);
    }

    public ICardDataService? PreviousPick
    {
        get => previousPick;
        private set => this.RaiseAndSetIfChanged(ref previousPick, value);
    }

    public void ClearCardsSeenAndDeck()
    {
        cardsSeen.Clear();
        Deck.Clear();
    }

    internal static void ClearOldDrafts()
    {
        string draftDirectory = GetDraftCacheDirectory();
        if (!Directory.Exists(draftDirectory))
        {
            return;
        }

        var filePaths = Directory.GetFiles(draftDirectory);
        foreach (var filePath in filePaths)
        {
            var lastAccessTime = File.GetLastAccessTimeUtc(filePath);
            var lastAccessTimeSpan = DateTime.UtcNow.Subtract(lastAccessTime);
            if (lastAccessTimeSpan.TotalDays >= CacheValidDays)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Logger.Error("Error removing card image {cardImageFilePath}. Exception: {exception}", filePath, ex);
                }
            }
        }
    }

    [MemberNotNull(nameof(cardDataSource))]
    internal void SetCardDataSource(ICardDataSource newCardDataSource)
    {
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
        ClearCardsSeenAndDeck();

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
        
        Logger.Info($"Current deck [{string.Join(",", draftState.GetCurrentDeck())}]");

        var missing = draftState.GetCurrentMissingCards();
        if (missing.Count > 0)
        {
            Logger.Info($"Missing cards [{string.Join(",", missing)}]");
        }

        var upcoming = draftState.GetCurrentUpcomingCards();
        if (upcoming.Count > 0)
        {
            Logger.Info($"Upcoming cards [{string.Join(",", upcoming)}]");
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
        
        Logger.Info($"Current deck [{string.Join(",", draftState.GetCurrentDeck())}]");
    }

    private void DraftLeaveHandler(object? sender, DraftLeaveEvent e)
    {
        cardsInCurrentPack.Clear();
        cardsMissingInCurrentPack.Clear();
        cardsUpcomingAfterCurrentPack.Clear();

        PreviousPick = null;

        draftState.ProcessEvent(e);
    }

    private void UpdateCardDataAfterChoice(ICardDataSource newCardDataSource)
    {
        cardsInCurrentPack.Clear();
        cardsMissingInCurrentPack.Clear();
        cardsUpcomingAfterCurrentPack.Clear();

        PreviousPick = null;

        foreach (var cardInPack in draftState.CurrentPack)
        {
            var cardInPackData = newCardDataSource.GetDataForCard(cardInPack, draftState);
            if (cardInPackData != null)
            {
                cardsInCurrentPack.Add(new CardDataModel(cardInPackData));
            }
            else
            {
                Logger.Info(string.Format($"Could not find card data for in pack card with MTGA id {cardInPack}"));
            }
        }

        foreach (var missingCard in draftState.GetCurrentMissingCards())
        {
            var missingCardData = newCardDataSource.GetDataForCard(missingCard, draftState);
            if (missingCardData != null)
            {
                cardsMissingInCurrentPack.Add(new CardDataModel(missingCardData));
            }
            else
            {
                Logger.Info(string.Format($"Could not find card data for missing card with MTGA id {missingCard}"));
            }
        }

        foreach (var upcomingCard in draftState.GetCurrentUpcomingCards())
        {
            var upcomingCardData = newCardDataSource.GetDataForCard(upcomingCard, draftState);
            if (upcomingCardData != null)
            {
                cardsUpcomingAfterCurrentPack.Add(new CardDataModel(upcomingCardData));
            }
            else
            {
                Logger.Info(string.Format($"Could not find card data for upcoming card with MTGA id {upcomingCard}"));
            }
        }

        var previouslyPickedCard = draftState.GetCurrentPackPreviousPick();
        if (previouslyPickedCard != null)
        {
            var previouslyPickedCardData = newCardDataSource.GetDataForCard(previouslyPickedCard.Value, draftState);
            if(previouslyPickedCardData != null)
            {
                PreviousPick = new CardDataModel(previouslyPickedCardData);
            }
        }
    }

    private void UpdateCardDataAfterPick(ICardDataSource newCardDataSource)
    {
        cardsSeen.Clear();
        Deck.Clear();

        var cardList = draftState.GetCurrentDeck()
                                 .Select(c => newCardDataSource.GetDataForCard(c, draftState))
                                 .Where(x => x != null)
                                 .Cast<ICardData>()
                                 .ToList();
        Deck.SetDeck(new DraftDeck(cardList));
        
        foreach (var seenCard in draftState.GetSeenCards())
        {
            var seenCardData = newCardDataSource.GetDataForCard(seenCard, draftState);
            if (seenCardData != null)
            {
                cardsSeen.Add(new CardDataModel(seenCardData));
            }
            else
            {
                Logger.Info(string.Format($"Could not find card data for seen card with MTGA id {seenCard}"));
            }
        }
    }

    private static void SerializeDraftState(DraftState state)
    {
        if(state.DraftId == Guid.Empty)
        {
            return;
        }

        string draftDirectory = GetDraftCacheDirectory();
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

    private static string GetDraftCacheDirectory()
    {
        string executableDirectory = Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".";
        string draftDirectory = Path.Combine(executableDirectory, "Drafts");
        return draftDirectory;
    }

    private readonly DraftState draftState;

    private const int CacheValidDays = 7;

    private readonly ObservableCollection<ICardDataService> cardsInCurrentPack;
    private readonly ObservableCollection<ICardDataService> cardsMissingInCurrentPack;
    private readonly ObservableCollection<ICardDataService> cardsUpcomingAfterCurrentPack;
    private readonly ObservableCollection<ICardDataService> cardsSeen;
    private IDeckService deck;
    private ICardDataService? previousPick;

    private readonly WatcherModel logModel;
    private ICardDataSource cardDataSource;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
}
