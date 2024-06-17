using Avalonia.Media.Imaging;
using System.Threading.Tasks;

namespace CardPile.App.ViewModels.Design;

public class CardViewModelDesign : ViewModelBase
{
    public CardViewModelDesign()
    { }

    internal string CardName
    {
        get => "Some Card";
    }

    internal Task<Bitmap?> CardImage { get => Task.FromResult<Bitmap?>(null); }
}
