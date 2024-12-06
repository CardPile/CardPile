using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CardPile.App.Services;
using CardPile.CardData.Importance;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPile.App.ViewModels;

public class CardDataViewModel : CardViewModelBase
{
    public const int CARD_HEADER_SIZE = 26;

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

    internal CardDataViewModel(ICardDataService cardDataService, int index, bool showLabel = true) : base(cardDataService, showLabel)
    {
        metrics = [.. CardDataService.Metrics.Select(x => new CardMetricViewModel(x))];

        Index = index;
    }

    internal string CardName
    { 
        get => CardDataService.Name; 
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
        get => CardDataService.CardImage; 
    }

    internal int Index
    {
        get => index;
        set
        {
            if (index != value)
            {
                this.RaisePropertyChanging(nameof(Index));
                this.RaisePropertyChanging(nameof(OffsetInStack));
                index = value;
                this.RaisePropertyChanged(nameof(OffsetInStack));
                this.RaisePropertyChanged(nameof(Index));
            }
        }
    }

    internal float OffsetInStack
    {
        get
        {
            return index * CARD_HEADER_SIZE;
        }
    }

    private readonly List<CardMetricViewModel> metrics;

    private int index;
}
