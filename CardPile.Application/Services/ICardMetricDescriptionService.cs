using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;

namespace CardPile.Application.Services;

internal interface ICardMetricDescriptionService : IReactiveObject
{
    public string Name { get; }

    public bool IsDefaultVisible { get; }

    public bool IsDefaultMetric { get; }

    public IComparer<ICardMetric> Comparer { get; }
}
