using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.Crypt;
using NLog;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardPile.Application.Models;

internal class CryptModel : ReactiveObject, ICryptService
{
    internal CryptModel(ICardDataSource cardDataSource)
    {
        Skeletons = [.. crypt.Skeletons.Select(s => new SkeletonModel(s, cardDataSource))];
    }

    public ObservableCollection<ISkeletonService> Skeletons { get; }

    public void SetCardDataSource(ICardDataSource cardDataSource)
    {
        foreach (var skeletonService in Skeletons)
        {
            if(skeletonService is not SkeletonModel skeleton)
            {
                logger.Error("ISkeletonService is not a SkeletonModel");
                continue;
            }

            skeleton.SetCardDataSource(cardDataSource);
        }
    }

    public void UpdateSkeletons(List<int> cardIds)
    {
        crypt.UpdateSkeletons(cardIds);

        foreach(var skeletonService in Skeletons)
        {
            if(skeletonService is not SkeletonModel skeletonModel)
            {
                logger.Error("ISkeletonService is not a SkeletonModel");
                continue;
            }

            skeletonModel.NotifyPropertiesChanged();
        }
    }

    public void Clear()
    {
        foreach (var skeletonService in Skeletons)
        {
            if (skeletonService is not SkeletonModel skeletonModel)
            {
                logger.Error("ISkeletonService is not a SkeletonModel");
                continue;
            }

            skeletonModel.ClearCount();
        }
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly Crypt.Crypt crypt = new();
}
