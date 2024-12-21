using System.Collections.Generic;

namespace CardPile.Application.ViewModels.Design;

public class CardMetricsViewModelDesign
{
    internal List<CardMetricViewModel> Metrics
    {
        get => [];
    }

    internal bool AnyMetricVisible
    {
        get => false;
    }
}