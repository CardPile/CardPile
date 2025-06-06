﻿using CardPile.CardData.Importance;
using CardPile.Crypt;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace CardPile.Application.Services;

internal interface ISkeletonCardGroupService : IReactiveObject
{
    public string Name { get; }

    public Range? Range { get; }

    public ImportanceLevel Importance { get; }

    public int Count { get; }

    public bool IsSatisfied { get; }

    public ObservableCollection<ISkeletonCardGroupService> Groups { get; }

    public ObservableCollection<ISkeletonCardEntryService> Cards { get; }
}
