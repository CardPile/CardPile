using System;
using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.CardData.Importance;
using NLog;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CardPile.Application.Models;

internal class CryptModel : ReactiveObject, ICryptService
{
    internal CryptModel(ICardDataSource cardDataSource)
    {
        this.cardDataSource = cardDataSource;
        
        ReloadSkeletons();

        Configuration.Instance.ObservableForProperty(x => x.CryptLocation).Subscribe(_ => ReloadSkeletons());
        Configuration.Instance.ObservableForProperty(x => x.ShowAllSkeletons).Subscribe(_ => ReloadSkeletons());
    }

    public ObservableCollection<ISkeletonService> Skeletons { get; } = [];

    public void SetCardDataSource(ICardDataSource newCardDataSource)
    {
        cardDataSource = newCardDataSource;
        UpdateSkeletonModels(Configuration.Instance.ShowAllSkeletons);
    }

    public void AnnotateCard(ICardDataService card)
    {
        foreach (var skeletonService in Skeletons)
        {
            if (skeletonService is not SkeletonModel skeleton)
            {
                logger.Error("ISkeletonService is not a SkeletonModel");
                continue;
            }

            var result = skeleton.CanAcceptCard(card);
            if(result != null)
            {
                var (importance, range) = result.Value;
                var name = string.Format("{0}In {1}", ImportanceUtils.ToMarker(importance), skeleton.Name);
                var text = range != null ? string.Format("{0}{1}", ImportanceUtils.ToMarker(importance), range.TextValue) : string.Empty;
                card.Annotations.Add(new CardAnnotationModel(name, text));
            }
        }
    }

    public void ReloadSkeletons()
    {
        crypt.LoadSkeletons(Configuration.Instance.CryptLocation);
        UpdateSkeletonModels(Configuration.Instance.ShowAllSkeletons);
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

    private void UpdateSkeletonModels(bool showAllSkeletons)
    {
        Skeletons.Clear();

        foreach (var skeleton in crypt.Skeletons)
        {
            if (!showAllSkeletons && cardDataSource.Set != null && cardDataSource.Set != skeleton.Set)
            {
                continue;
            }

            Skeletons.Add(new SkeletonModel(skeleton, cardDataSource));
        }
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly Crypt.Crypt crypt = new();

    private ICardDataSource cardDataSource;
}
