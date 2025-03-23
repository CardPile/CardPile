using Avalonia.Data.Converters;
using Avalonia.Media;
using CardPile.CardData.Importance;
using System.Collections.Generic;
using System.Linq;

namespace CardPile.Application.ViewModels;

internal class CardMetricsViewModel
{
    public static FuncValueConverter<ImportanceLevel, IBrush> ImportanceConverter { get; } = new FuncValueConverter<ImportanceLevel, IBrush>(level => ConverterUtils.ImportanceToBrush(level));

    internal CardMetricsViewModel(List<CardMetricViewModel> metrics)
    {
        this.metrics = metrics;
    }

    internal List<CardMetricViewModel> Metrics
    {
        get => metrics;
    }

    internal bool AnyMetricVisible
    {
        get => metrics.Aggregate(false, (acc, x) => acc || (x.Metric != null && x.Metric.HasValue)) && metrics.Aggregate(false, (acc, x) => acc || x.Visible);
    }

    private readonly List<CardMetricViewModel> metrics;
}
