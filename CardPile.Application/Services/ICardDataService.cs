using Avalonia.Media.Imaging;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardPile.Application.Services;

public interface ICardDataService : IReactiveObject
{
    public string Name { get; }

    public int ArenaCardId { get; }

    public Color Colors { get; }

    public string? Url { get; }

    public List<ICardMetric> Metrics { get; }

    public List<ICardAnnotationService> Annotations { get; }

    public Task<Bitmap?> CardImage { get; }

    public Task<Bitmap?> StandardCardImage { get; }
}
