using Avalonia;
using Avalonia.Controls;

namespace CardPile.Application.Views;

public partial class CardListView : UserControl
{
    public static readonly DirectProperty<CardListView, bool> ShowMetricsProperty = AvaloniaProperty.RegisterDirect<CardListView, bool>(
    nameof(ShowMetrics),
    o => o.ShowMetrics,
    (o, v) => o.ShowMetrics = v
);

    public static readonly DirectProperty<CardListView, bool> ShowPopupProperty = AvaloniaProperty.RegisterDirect<CardListView, bool>(
        nameof(ShowPopup),
        o => o.ShowPopup,
        (o, v) => o.ShowPopup = v
    );

    public static readonly DirectProperty<CardListView, bool> CanBeDeactivatedProperty = AvaloniaProperty.RegisterDirect<CardListView, bool>(
        nameof(CanBeDeactivated),
        o => o.CanBeDeactivated,
        (o, v) => o.CanBeDeactivated = v
    );

    public CardListView()
    {
        InitializeComponent();
    }

    public bool ShowMetrics
    {
        get => showMetrics;
        set => SetAndRaise(ShowMetricsProperty, ref showMetrics, value);
    }

    public bool ShowPopup
    {
        get => showPopup;
        set => SetAndRaise(ShowPopupProperty, ref showPopup, value);
    }

    public bool CanBeDeactivated
    {
        get => canBeDeactivated;
        set => SetAndRaise(CanBeDeactivatedProperty, ref canBeDeactivated, value);
    }

    private bool showMetrics = true;
    private bool showPopup = false;
    private bool canBeDeactivated = false;
}