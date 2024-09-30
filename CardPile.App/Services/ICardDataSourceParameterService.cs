using CardPile.CardData.Parameters;
using System.Reflection.Metadata;

namespace CardPile.App.Services;

internal interface ICardDataSourceParameterService
{
    public string Name { get; }

    public ParameterType Type { get; }
}
