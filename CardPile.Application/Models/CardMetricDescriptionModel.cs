using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;

namespace CardPile.Application.Models;

internal class CardMetricDescriptionModel : ReactiveObject, ICardMetricDescriptionService
{
    internal CardMetricDescriptionModel(ICardMetricDescription cardMetricDescription)
    {
        this.cardMetricDescription = cardMetricDescription;
    }

    public string Name => cardMetricDescription.Name;

    public bool IsDefaultVisible { get => cardMetricDescription.IsDefaultVisible; }

    public bool IsDefaultMetric { get => cardMetricDescription.IsDefaultMetric; }

    public IComparer<ICardMetric> Comparer => cardMetricDescription.Comparer;

    private ICardMetricDescription cardMetricDescription;
}
