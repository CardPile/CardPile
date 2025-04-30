using ReactiveUI;
using System.Collections.ObjectModel;

namespace CardPile.Application.Services;

internal interface ICryptService : IReactiveObject
{
    public ObservableCollection<ISkeletonService> Skeletons { get; }
}
