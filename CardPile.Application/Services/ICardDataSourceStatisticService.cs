using ReactiveUI;

namespace CardPile.Application.Services;

internal interface ICardDataSourceStatisticService : IReactiveObject
{
    public string Name { get; }

    public string TextValue { get; }
}
