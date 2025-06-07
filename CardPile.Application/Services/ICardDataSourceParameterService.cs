using CardPile.CardData.Parameters;
using ReactiveUI;
using System.Reflection.Metadata;

namespace CardPile.Application.Services;

internal interface ICardDataSourceParameterService : IReactiveObject
{
    public string Name { get; }

    public ParameterType Type { get; }
}
