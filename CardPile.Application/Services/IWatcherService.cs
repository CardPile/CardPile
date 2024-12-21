using CardPile.Draft;
using ReactiveUI;
using System;

namespace CardPile.Application.Services;

internal interface IWatcherService : IReactiveObject
{
    public event EventHandler<DraftEnterEvent>? DraftEnterEvent;
    public event EventHandler<DraftChoiceEvent>? DraftChoiceEvent;
    public event EventHandler<DraftPickEvent>? DraftPickEvent;
    public event EventHandler<DraftLeaveEvent>? DraftLeaveEvent;
}
