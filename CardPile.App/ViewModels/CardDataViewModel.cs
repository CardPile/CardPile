using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CardPile.App.Services;
using CardPile.CardData.Importance;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPile.App.ViewModels;

public class CardDataViewModel : ViewModelBase
{
    public static FuncValueConverter<ImportanceLevel, IBrush> ImportanceConverter { get; } = new FuncValueConverter<ImportanceLevel, IBrush>(level =>
    {
        switch(level)
        {
            case ImportanceLevel.Low: return new SolidColorBrush(Colors.LightGray);
            case ImportanceLevel.Regular:
            {
                if(!Avalonia.Application.Current!.TryGetResource("SystemControlForegroundBaseHighBrush", Avalonia.Application.Current.ActualThemeVariant, out object? foregroundBrush))
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

    internal CardDataViewModel(ICardDataService cardDataService)
    {
        this.cardDataService = cardDataService;
        this.metrics = [.. this.cardDataService.Metrics.Select(x => new CardMetricViewModel(x))];
    }

    internal ICardDataService CardDataService
    { 
        get => cardDataService;
    }

    internal string CardName
    { 
        get => cardDataService.Name; 
    }

    internal List<CardMetricViewModel> Metrics
    {
        get => metrics;
    }

    internal bool AnyMetricToShow
    {
        get => metrics.Aggregate(false, (acc, x) => acc || (x.Metric != null && x.Metric.HasValue)) && metrics.Aggregate(false, (acc, x) => acc || x.Visible);
    }

    internal Task<Bitmap?> CardImage
    { 
        get => cardDataService.CardImage; 
    }

    private readonly ICardDataService cardDataService;
    private readonly List<CardMetricViewModel> metrics;
}
