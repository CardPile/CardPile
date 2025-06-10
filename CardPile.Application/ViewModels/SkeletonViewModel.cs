using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using CardPile.Application.Services;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;

namespace CardPile.Application.ViewModels;

internal class SkeletonViewModel : ViewModelBase
{
    public static FuncValueConverter<string, InlineCollection> TextToInlinesConverter { get; } = new FuncValueConverter<string, InlineCollection>(ConverterUtils.TextToInlines);

    internal SkeletonViewModel(ISkeletonService skeletonService)
    {
        SkeletonService = skeletonService;

        Groups = [.. skeletonService.Groups.Select(g => new SkeletonCardGroupViewModel(g))];
    }

    internal ISkeletonService SkeletonService { get; init; }

    internal string Name {  get => SkeletonService.Name; }

    internal string Set { get => SkeletonService.Set; }

    internal string Desc { get => SkeletonService.Desc; }

    internal ObservableCollection<SkeletonCardGroupViewModel> Groups { get; } = [];

    internal void UpdateCardMetricVisibility(Action<CardDataViewModel> updater)
    {
        foreach (var group in Groups)
        {
            group.UpdateCardMetricVisibility(updater);
        }
    }

    internal void UpdateCountVisibility(bool visible)
    {
        foreach (var group in Groups)
        {
            group.UpdateCountVisibility(visible);
        }
    }
}
