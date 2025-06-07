using Avalonia.Media.Imaging;
using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardPile.CardInfo;

namespace CardPile.Application.Models;

public class CardDataModel : ReactiveObject, ICardDataService
{
    public const int CARD_IMAGE_WIDTH = 172;
    public const int CARD_IMAGE_HEIGHT = 240;
    public const int CARD_HEADER_SIZE = 26;

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

    public Color Colors
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

    public List<ICardAnnotationService> Annotations { get; } = [];

    public Task<Bitmap?> CardImage
    {
        get => Scryfall.LoadBitmap($"{ArenaCardId}", Url); 
    }

    public Task<Bitmap?> StandardCardImage
    {
        get => Scryfall.LoadBitmapWithSize($"{ArenaCardId}", Url, CARD_IMAGE_WIDTH, CARD_IMAGE_HEIGHT);
    }

    private readonly ICardData cardData;
}
