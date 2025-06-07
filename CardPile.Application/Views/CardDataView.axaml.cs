using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

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

    public static readonly DirectProperty<CardDataView, bool> CanBeDeactivatedProperty = AvaloniaProperty.RegisterDirect<CardDataView, bool>(
        nameof(CanBeDeactivated),
        o => o.CanBeDeactivated,
        (o, v) => o.CanBeDeactivated = v
    );

    public static readonly DirectProperty<CardDataView, bool> ActiveProperty = AvaloniaProperty.RegisterDirect<CardDataView, bool>(
        nameof(Active),
        o => o.Active,
        (o, v) => o.Active = v
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

    public bool CanBeDeactivated
    {
        get => canBeDeactivated;
        set => SetAndRaise(CanBeDeactivatedProperty, ref canBeDeactivated, value);
    }

    public bool Active
    {
        get => active;
        set => SetAndRaise(ActiveProperty, ref active, value);
    }

    private void HandlePointerReleased(object sender, RoutedEventArgs e)
    {
        if(!canBeDeactivated)
        {
            return;
        }
        Active = !active;
    }

    private bool showLabel = true;
    private bool showMetrics = true;
    private bool showPopup = false;
    private bool canBeDeactivated = false;
    private bool active = true;
}