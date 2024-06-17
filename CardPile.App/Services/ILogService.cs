using ReactiveUI;

namespace CardPile.App.Services;

internal interface ILogService : IReactiveObject
{
    public string LogContents { get; }
}
