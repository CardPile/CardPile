﻿using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardPile.App.ViewModels.Design;

public class CardDataViewModelDesign : ViewModelBase
{
    public CardDataViewModelDesign()
    {}

    internal string CardName
    {
        get => "Some Card";
    }

    internal List<CardMetricViewModel> Metrics
    {
        get => [];
    }

    internal bool AnyMetricIsNotNull
    {
        get => true;
    }

    internal Task<Bitmap?> CardImage { get => Task.FromResult<Bitmap?>(null); }
}
