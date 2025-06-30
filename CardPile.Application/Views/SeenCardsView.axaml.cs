using Avalonia;
using Avalonia.Controls;

namespace CardPile.Application.Views;

public partial class SeenCardsView : UserControl
{
    public static readonly DirectProperty<SeenCardsView, bool> MergePacksProperty = AvaloniaProperty.RegisterDirect<SeenCardsView, bool>(
        nameof(MergePacks),
        o => o.MergePacks,
        (o, v) => o.MergePacks = v
    );

    public static readonly DirectProperty<SeenCardsView, bool> ReversePacksProperty = AvaloniaProperty.RegisterDirect<SeenCardsView, bool>(
        nameof(ReversePacks),
        o => o.ReversePacks,
        (o, v) => o.ReversePacks = v
    );
    
    public static readonly DirectProperty<SeenCardsView, bool> StackPacksProperty = AvaloniaProperty.RegisterDirect<SeenCardsView, bool>(
        nameof(StackPacks),
        o => o.StackPacks,
        (o, v) => o.StackPacks = v
    );

    public SeenCardsView()
    {
        InitializeComponent();
    }

    public bool MergePacks
    {
        get => mergePacks;
        set => SetAndRaise(MergePacksProperty, ref mergePacks, value);
    }

    public bool ReversePacks
    {
        get => reversePacks;
        set => SetAndRaise(ReversePacksProperty, ref reversePacks, value);
    }    
    
    public bool StackPacks
    {
        get => stackPacks;
        set => SetAndRaise(StackPacksProperty, ref stackPacks, value);
    }

    private bool mergePacks = false;
    private bool reversePacks = true;
    private bool stackPacks = false;
}