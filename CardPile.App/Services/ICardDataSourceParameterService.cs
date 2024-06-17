using CardPile.CardData;
using System.Reflection.Metadata;

namespace CardPile.App.Services;

internal interface ICardDataSourceParameterService
{
    public string Name { get; }

    public CardDataSourceParameterType Type { get; }
}
