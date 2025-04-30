using Avalonia;
using Avalonia.Controls;

namespace CardPile.Application.Views;

public partial class HorizontalCardListView : UserControl
{
    public static readonly DirectProperty<HorizontalCardListView, bool> ShowMetricsProperty = AvaloniaProperty.RegisterDirect<HorizontalCardListView, bool>(
    nameof(ShowMetrics),
    o => o.ShowMetrics,
    (o, v) => o.ShowMetrics = v
);

    public static readonly DirectProperty<HorizontalCardListView, bool> ShowPopupProperty = AvaloniaProperty.RegisterDirect<HorizontalCardListView, bool>(
        nameof(ShowPopup),
        o => o.ShowPopup,
        (o, v) => o.ShowPopup = v
    );

    public static readonly DirectProperty<HorizontalCardListView, bool> CanBeDeactivatedProperty = AvaloniaProperty.RegisterDirect<HorizontalCardListView, bool>(
        nameof(CanBeDeactivated),
        o => o.CanBeDeactivated,
        (o, v) => o.CanBeDeactivated = v
    );

    public HorizontalCardListView()
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