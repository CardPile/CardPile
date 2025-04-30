using CardPile.Application.Services;
using CardPile.CardData;
using NLog;
using ReactiveUI;
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

    internal void SetCardDataSource(ICardDataSource cardDataSource)
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

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly Crypt.Crypt crypt = new();
}
