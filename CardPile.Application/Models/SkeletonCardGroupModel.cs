using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.CardData.Importance;
using CardPile.Crypt;
using NLog;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardPile.Application.Models;

internal class SkeletonCardGroupModel : ReactiveObject, ISkeletonCardGroupService
{
    public SkeletonCardGroupModel(CardGroup cardGroup, ICardDataSource cardDataSource)
    {
        CardGroup = cardGroup;

        Groups = [.. cardGroup.Groups.Select(g => new SkeletonCardGroupModel(g, cardDataSource))];

        Cards = [.. cardGroup.Cards.Select(c => (cardDataSource.GetDataForCard(c.PrimaryCardId), c))
                                   .Where(x => x.Item1 != null)
                                   .Cast<(ICardData, CardEntry)>()
                                   .Select(c => new SkeletonCardEntryModel(c.Item1, c.Item2))];
    }

    public CardGroup CardGroup { get; init; }

    public string Name { get => CardGroup.Name; }

    public ImportanceLevel Importance { get => CardGroup.Importance; }

    public Range Range { get => CardGroup.Range; } 

    public ObservableCollection<ISkeletonCardGroupService> Groups { get; }

    public ObservableCollection<ISkeletonCardEntryService> Cards { get; }

    internal void SetCardDataSource(ICardDataSource cardDataSource)
    { 
        foreach(var groupService in Groups)
        {
            if(groupService is not  SkeletonCardGroupModel group)
            {
                logger.Error("ISkeletonCardGroupService is not a SkeletonCardGroupModel");
                continue;
            }

            group.SetCardDataSource(cardDataSource);
        }

        var cardList = CardGroup.Cards.Select(c => (cardDataSource.GetDataForCard(c.PrimaryCardId), c))
                                      .Where(x => x.Item1 != null)
                                      .Cast<(ICardData, CardEntry)>();

        Cards.Clear();
        foreach (var (card, cardEntry) in cardList)
        {
            Cards.Add(new SkeletonCardEntryModel(card, cardEntry));
        }
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
