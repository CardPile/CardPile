using Avalonia.Media.Imaging;
using CardPile.Application.Models;
using CardPile.Application.Services;
using ReactiveUI;
using System.Linq;
using System.Threading.Tasks;

namespace CardPile.Application.ViewModels;

public class CardDataViewModel : ViewModelBase
{
    internal CardDataViewModel(ICardDataService cardDataService, int index, bool highlight = false)
    {
        CardDataService = cardDataService;
        Metrics = new CardMetricsViewModel([.. CardDataService.Metrics.Select(x => new CardMetricViewModel(x))]);
        Annotations = new CardAnnotationsViewModel([.. CardDataService.Annotations.Select(x => new CardAnnotationViewModel(x))]);
        Index = index;
        Highlight = highlight;
    }

    internal ICardDataService CardDataService { get; init; }

    internal string CardName
    { 
        get => CardDataService.Name; 
    }

    internal CardMetricsViewModel Metrics { get; init; }

    internal CardAnnotationsViewModel Annotations { get; init; }

    internal Task<Bitmap?> CardImage
    { 
        get => CardDataService.CardImage; 
    }

    internal Task<Bitmap?> StandardCardImage
    {
        get => CardDataService.StandardCardImage;
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
            return index * CardDataModel.CARD_HEADER_SIZE;
        }
    }
    
    internal bool Highlight { get; init; }

    private int index;
}
