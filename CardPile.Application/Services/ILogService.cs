using ReactiveUI;

namespace CardPile.Application.Services;

internal interface ILogService : IReactiveObject
{
    public string LogContents { get; }
}
