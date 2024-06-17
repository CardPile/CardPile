using Avalonia.Media.Imaging;
using CardPile.App.Services;
using System.Threading.Tasks;

namespace CardPile.App.ViewModels;

public class CardViewModel : ViewModelBase
{
    internal CardViewModel(ICardDataService cardDataService, bool highlight = false)
    {
        this.cardDataService = cardDataService;
        Highlight = highlight;
    }

    internal ICardDataService CardDataService
    {
        get => cardDataService;
    }

    internal string CardName
    {
        get => cardDataService.Name;
    }

    internal Task<Bitmap?> CardImage
    { 
        get => cardDataService.CardImage; 
    }

    internal bool Highlight { get; init; }

    private readonly ICardDataService cardDataService;
}
