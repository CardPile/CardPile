using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardPile.Application.ViewModels.Design;

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

    internal bool AnyMetricToShow
    {
        get => true;
    }

    internal Task<Bitmap?> CardImage { get => Task.FromResult<Bitmap?>(null); }
}
