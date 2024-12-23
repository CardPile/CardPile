using Avalonia;
using Avalonia.Controls;

namespace CardPile.Application.Views;

public partial class CardDataView : UserControl
{
    public static readonly DirectProperty<CardDataView, bool> ShowLabelProperty = AvaloniaProperty.RegisterDirect<CardDataView, bool>(
        nameof(ShowLabel),
        o => o.ShowLabel,
        (o, v) => o.ShowLabel = v
    );
    
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

    public bool ShowLabel
    {
        get => showLabel;
        set => SetAndRaise(ShowLabelProperty, ref showLabel, value);
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

    private bool showLabel = true;
    private bool showMetrics = true;
    private bool showPopup = false;
}