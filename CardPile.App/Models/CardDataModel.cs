using Avalonia.Media.Imaging;
using CardPile.App.Services;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardPile.CardInfo;

namespace CardPile.App.Models;

internal class CardDataModel : ReactiveObject, ICardDataService
{
    public const int CARD_IMAGE_WIDTH = 172;
    
    internal CardDataModel(ICardData cardData)
    {
        this.cardData = cardData;
    }
    
    public string Name
    {
        get => cardData.Name;
    }

    public int ArenaCardId
    {
        get => cardData.ArenaCardId;
    }

    public List<Color> Colors
    {
        get => cardData.Colors;
    }

    public string? Url
    { 
        get => cardData.Url; 
    }

    public List<ICardMetric> Metrics
    { 
        get => cardData.Metrics; 
    }

    public Task<Bitmap?> CardImage
    {
        get => Scryfall.LoadBitmap($"{ArenaCardId}", Url); 
    }
    
    private readonly ICardData cardData;
}
