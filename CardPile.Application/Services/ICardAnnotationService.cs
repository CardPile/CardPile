using ReactiveUI;

namespace CardPile.Application.Services;

public interface ICardAnnotationService : IReactiveObject
{
    public string Name { get; }

    public string TextValue { get; }
}
