using CardPile.App.Services;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;

namespace CardPile.App.ViewModels;

public class CardDataMetricDescriptionViewModel : ViewModelBase
{
    internal CardDataMetricDescriptionViewModel(ICardMetricDescriptionService cardMetricDescriptionService) : this(cardMetricDescriptionService, false)
    { }

    internal CardDataMetricDescriptionViewModel(ICardMetricDescriptionService cardMetricDescriptionService, bool visible)
    {
        this.cardMetricDescriptionService = cardMetricDescriptionService;
        this.visible = visible;
    }

    public string Name
    {
        get => cardMetricDescriptionService.Name;
    }

    public IComparer<ICardMetric> Comparer
    { 
        get => cardMetricDescriptionService.Comparer; 
    }

    public bool Visible
    { 
        get => visible;
        set => this.RaiseAndSetIfChanged(ref visible, value);
    }

    private ICardMetricDescriptionService cardMetricDescriptionService;
    private bool visible;
}
