using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using System.Collections.Generic;

namespace CardPile.Application.ViewModels;

internal class CardAnnotationsViewModel
{
    internal CardAnnotationsViewModel(List<CardAnnotationViewModel> annotations)
    {
        this.annotations = annotations;
    }

    internal List<CardAnnotationViewModel> Annotations
    {
        get => annotations;
    }

    internal bool AnyAnnotationVisible
    {
        get => annotations.Count > 0;
    }

    private readonly List<CardAnnotationViewModel> annotations;
}
