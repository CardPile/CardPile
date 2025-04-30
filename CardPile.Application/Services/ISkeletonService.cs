using CardPile.Application.Models;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace CardPile.Application.Services;

internal interface ISkeletonService : IReactiveObject
{
    public string Name { get; }

    public string Set { get; }

    public ObservableCollection<ISkeletonCardGroupService> Groups { get; }
}
