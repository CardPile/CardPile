using Avalonia;
using Avalonia.Controls;

namespace CardPile.App.Views;

public partial class CardMetricsView : UserControl
{
    public static readonly StyledProperty<bool> HighlightImportanceProperty = AvaloniaProperty.Register<CardMetricsView, bool>(nameof(HighlightImportance), defaultValue: true);

    public CardMetricsView()
    {
        InitializeComponent();
    }

    public bool HighlightImportance
    {
        get => GetValue(HighlightImportanceProperty);
        set => SetValue(HighlightImportanceProperty, value);
    }
}