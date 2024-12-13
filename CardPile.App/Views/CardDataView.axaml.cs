using Avalonia;
using Avalonia.Controls;

namespace CardPile.App.Views;

public partial class CardDataView : UserControl
{
    public static readonly DirectProperty<CardDataView, bool> ShowMetricsProperty = AvaloniaProperty.RegisterDirect<CardDataView, bool>(
        nameof(ShowMetrics),
        o => o.ShowMetrics,
        (o, v) => o.ShowMetrics = v
    );

    public static readonly DirectProperty<CardDataView, bool> ShowPopupProperty = AvaloniaProperty.RegisterDirect<CardDataView, bool>(
        nameof(ShowPopup),
        o => o.ShowPopup,
        (o, v) => o.ShowPopup = v
    );

    public CardDataView()
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

    private bool showMetrics = true;
    private bool showPopup = false;
}