using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.Crypt;
using NLog;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace CardPile.Application.Models;

internal class SkeletonModel : ReactiveObject, ISkeletonService
{
    public SkeletonModel(Skeleton skeleton, ICardDataSource cardDataSource)
    {
        Skeleton = skeleton;

        Groups = [.. skeleton.Groups.Select(g => new SkeletonCardGroupModel(g, cardDataSource))];
    }

    public Skeleton Skeleton { get; init; }

    public string Name { get => Skeleton.Name; }

    public string Set { get => Skeleton.Set; }

    public ObservableCollection<ISkeletonCardGroupService> Groups { get; init; }

    internal void SetCardDataSource(ICardDataSource cardDataSource)
    {
        foreach (var groupService in Groups)
        {
            if(groupService is not SkeletonCardGroupModel group)
            {
                logger.Error("ISkeletonCardGroupService is not a SkeletonCardGroupModel");
                continue;
            }

            group.SetCardDataSource(cardDataSource);
        }
    }

    internal void NotifyPropertiesChanged()
    {
        foreach(var groupService in Groups)
        {
            if(groupService is not SkeletonCardGroupModel cardGroupModel)
            {
                logger.Error("ISkeletonCardGroupService is not a SkeletonCardGroupModel");
                continue;
            }

            cardGroupModel.NotifyPropertiesChanged();
        }
    }

    internal void ClearCount()
    {
        Skeleton.ClearCount();
        NotifyPropertiesChanged();
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
