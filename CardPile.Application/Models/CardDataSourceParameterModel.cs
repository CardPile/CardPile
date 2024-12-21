using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.CardData.Parameters;
using ReactiveUI;

namespace CardPile.Application.Models;

internal class CardDataSourceParameterModel : ReactiveObject, ICardDataSourceParameterService
{
    internal CardDataSourceParameterModel(ICardDataSourceParameter parameter)
    {
        this.parameter = parameter;
    }

    public string Name { get => parameter.Name; }

    public ParameterType Type { get => parameter.Type; }

    protected T? As<T>() where T : class, ICardDataSourceParameter
    {
        return parameter as T;
    }

    private ICardDataSourceParameter parameter;
}
