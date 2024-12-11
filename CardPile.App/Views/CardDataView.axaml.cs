using Avalonia;
using Avalonia.Controls;

namespace CardPile.App.Views;

public partial class CardDataView : UserControl
{
    public static readonly StyledProperty<bool> ShowMetricsProperty = AvaloniaProperty.Register<CardDataView, bool>(nameof(ShowMetrics), defaultValue: true);

    public static readonly StyledProperty<bool> ShowPopupProperty = AvaloniaProperty.Register<CardDataView, bool>(nameof(ShowMetrics), defaultValue: false);

    public CardDataView()
    {
        InitializeComponent();
    }

    public bool ShowMetrics
    {
        get => GetValue(ShowMetricsProperty);
        set => SetValue(ShowMetricsProperty, value);
    }

    public bool ShowPopup
    {
        get => GetValue(ShowPopupProperty);
        set => SetValue(ShowPopupProperty, value);
    }
}