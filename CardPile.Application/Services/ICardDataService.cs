using Avalonia.Media.Imaging;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardPile.Application.Services;

internal interface ICardDataService : IReactiveObject
{
    public string Name { get; }

    public int ArenaCardId { get; }

    public List<Color> Colors { get; }

    public string? Url { get; }

    public List<ICardMetric> Metrics { get; }

    public Task<Bitmap?> CardImage { get; }
}
