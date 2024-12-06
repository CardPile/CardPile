using Avalonia.Media.Imaging;
using CardPile.App.Services;
using System.Threading.Tasks;

namespace CardPile.App.ViewModels;

public class CardViewModel : CardViewModelBase
{
    internal CardViewModel(ICardDataService cardDataService, bool highlight = false, bool showLabel = true) : base(cardDataService, showLabel)
    {
        Highlight = highlight;
    }

    internal string CardName
    {
        get => CardDataService.Name;
    }

    internal Task<Bitmap?> CardImage
    { 
        get => CardDataService.CardImage; 
    }

    internal bool Highlight { get; init; }
}
