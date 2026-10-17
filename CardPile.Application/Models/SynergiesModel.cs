using System;
using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.CardData.Importance;
using CardPile.Draft;
using CardPile.Synergies;
using NLog;
using ReactiveUI;
using System.Linq;


namespace CardPile.Application.Models;

internal class SynergiesModel : ReactiveObject, ISynergiesService
{
    internal SynergiesModel(ICardDataSource cardDataSource, DraftState draftState)
    {
        this.cardDataSource = cardDataSource;
        this.draftState = draftState;

        ReloadSynergies();

        Configuration.Instance.ObservableForProperty(x => x.SynergiesLocation).Subscribe(_ => ReloadSynergies());
    }

    public void SetCardDataSource(ICardDataSource newCardDataSource)
    {
        cardDataSource = newCardDataSource;
    }

    public void AnnotateCard(ICardDataService card)
    {
        if (cardDataSource.Set == null)
        {
            return;
        }

        if (!synergies.SynergySets.TryGetValue(cardDataSource.Set, out var synergySet))
        {
            return;
        }

        var deckCardList = draftState.GetCurrentDeck()
                                     .Distinct()
                                     .Select(c => cardDataSource.GetDataForCard(c, draftState))
                                     .Where(x => x != null)
                                     .Cast<ICardData>()
                                     .ToList();

        foreach (var deckCard in deckCardList)
        {
            var synergy = synergySet.GetSynergy(card.Name, deckCard.Name);
            if (synergy == null)
            {
                continue;
            }

            var importance = GetImportanceForStrength(synergy.Strength);
            var name = $"{ImportanceUtils.ToMarker(importance)}Syn. with {deckCard.Name}";
            var text = $"{ImportanceUtils.ToMarker(importance)}{synergy.Reason}";
            card.Annotations.Add(new CardAnnotationModel(name, text));
        }
    }

    public void ReloadSynergies()
    {
        synergies.LoadSynergies(Configuration.Instance.SynergiesLocation);
    }

    private ImportanceLevel GetImportanceForStrength(SynergyStrength strength)
    {
        switch (strength)
        {
            case SynergyStrength.None:
                return ImportanceLevel.Low;
            case SynergyStrength.Weak:
                return ImportanceLevel.Low;
            case SynergyStrength.Medium:
                return ImportanceLevel.Regular;
            case SynergyStrength.Strong:
                return ImportanceLevel.Critical;
            case SynergyStrength.Full:
                return ImportanceLevel.Critical;
            default:
                throw new ArgumentOutOfRangeException(nameof(strength), strength, null);
        }
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly Synergies.SynergyProvider synergies = new();

    private ICardDataSource cardDataSource;
    private DraftState draftState;
}

