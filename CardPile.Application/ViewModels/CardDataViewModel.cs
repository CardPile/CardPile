using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CardPile.Application.Services;
using CardPile.CardData.Importance;
using ReactiveUI;
using System.Linq;
using System.Threading.Tasks;

namespace CardPile.Application.ViewModels;

public class CardDataViewModel : CardViewModelBase
{
    public const int CARD_HEADER_SIZE = 26;

    internal CardDataViewModel(ICardDataService cardDataService, int index) : base(cardDataService)
    {
        Metrics = new CardMetricsViewModel([.. CardDataService.Metrics.Select(x => new CardMetricViewModel(x))]);
        Index = index;
    }

    internal string CardName
    { 
        get => CardDataService.Name; 
    }

    internal CardMetricsViewModel Metrics { get; init; }

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

    private int index;
}
