using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using CardPile.CardData;
using ReactiveUI;

namespace CardPile.Application.ViewModels;

internal class CardMetricViewModel : ViewModelBase
{
    public static FuncValueConverter<string, InlineCollection> MetricTextToInlinesConverter { get; } = new FuncValueConverter<string, InlineCollection>(text => ConverterUtils.TextToInlines(text));

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
