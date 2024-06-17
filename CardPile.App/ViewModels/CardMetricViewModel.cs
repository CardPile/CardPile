using CardPile.CardData;
using ReactiveUI;
using System.Data;

namespace CardPile.App.ViewModels;

internal class CardMetricViewModel : ViewModelBase
{
    internal CardMetricViewModel(ICardMetric cardMetric)
    {
        this.cardMetric = cardMetric;
    }

    internal ICardMetric Metric
    {
        get => cardMetric;
    }

    internal bool Visible
    {
        get => visible;
        set => this.RaiseAndSetIfChanged(ref visible, value);
    }

    private ICardMetric cardMetric;
    private bool visible;
}
