using CardPile.Draft;
using ReactiveUI;
using System;

namespace CardPile.App.Services;

internal interface IWatcherService : IReactiveObject
{
    public event EventHandler<DraftEnterEvent>? DraftStartEvent;
    public event EventHandler<DraftChoiceEvent>? DraftChoiceEvent;
    public event EventHandler<DraftPickEvent>? DraftPickEvent;
    public event EventHandler<DraftLeaveEvent>? DraftLeaveEvent;
}
