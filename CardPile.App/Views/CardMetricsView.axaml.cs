using Avalonia;
using Avalonia.Controls;

namespace CardPile.App.Views;

public partial class CardMetricsView : UserControl
{
    public static readonly DirectProperty<CardMetricsView, bool> HighlightImportanceProperty = AvaloniaProperty.RegisterDirect<CardMetricsView, bool>(
        nameof(HighlightImportance),
        o => o.HighlightImportance,
        (o, v) => o.HighlightImportance = v
    );

    public CardMetricsView()
    {
        InitializeComponent();
    }

    public bool HighlightImportance
    {
        get => highlightImportance;
        set => SetAndRaise(HighlightImportanceProperty, ref highlightImportance, value);
    }

    private bool highlightImportance;
}