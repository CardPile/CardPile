using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using CardPile.CardData;
using ReactiveUI;

namespace CardPile.Application.ViewModels;

internal class CardMetricViewModel : ViewModelBase
{
    public static FuncValueConverter<string, InlineCollection> MetricTextToInlinesConverter { get; } = new FuncValueConverter<string, InlineCollection>(text => ConverterUtils.TextToInlines(text));

    public static FuncValueConverter<ICardRank, InlineCollection> RankTextToInlinesConverter { get; } = new FuncValueConverter<ICardRank, InlineCollection>(rank =>
    {
        if(rank == null)
        {
            return [];
        }

        var result = ConverterUtils.TextToInlines(rank.Name);
        result.Insert(0, new Run { Text = $"#{rank.Value} in " });
        return result;
    });

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

    internal string TextValue
    {
        get => cardMetric.TextValue;
    }

    private ICardMetric cardMetric;
    private bool visible;
}
