using Avalonia.Data.Converters;
using Avalonia.Media;
using CardPile.CardData.Importance;
using System.Collections.Generic;
using System.Linq;

namespace CardPile.Application.ViewModels;

internal class CardMetricsViewModel
{
    public static FuncValueConverter<ImportanceLevel, IBrush> ImportanceConverter { get; } = new FuncValueConverter<ImportanceLevel, IBrush>(level =>
    {
        switch (level)
        {
            case ImportanceLevel.Low: return new SolidColorBrush(Colors.LightGray);
            case ImportanceLevel.Regular:
                {
                    if (!Avalonia.Application.Current!.TryGetResource("SystemControlForegroundBaseHighBrush", Avalonia.Application.Current.ActualThemeVariant, out object? foregroundBrush))
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                    var result = foregroundBrush as IBrush;
                    return result ?? new SolidColorBrush(Colors.White);
                }
            case ImportanceLevel.High: return new SolidColorBrush(Colors.Orange);
            case ImportanceLevel.Critical: return new SolidColorBrush(Colors.Red);
            default: throw new System.NotImplementedException();
        }
    });

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
