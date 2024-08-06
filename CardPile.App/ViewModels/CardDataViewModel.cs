using Avalonia.Media.Imaging;
using CardPile.App.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPile.App.ViewModels;

public class CardDataViewModel : ViewModelBase
{
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
