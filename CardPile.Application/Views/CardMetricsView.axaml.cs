using Avalonia;
using Avalonia.Controls;

namespace CardPile.Application.Views;

public partial class CardMetricsView : UserControl
{
    public static readonly DirectProperty<CardMetricsView, bool> HighlightImportanceProperty = AvaloniaProperty.RegisterDirect<CardMetricsView, bool>(
        nameof(HighlightImportance),
        o => o.HighlightImportance,
        (o, v) => o.HighlightImportance = v
    );

    public static readonly DirectProperty<CardMetricsView, bool> ShowRanksProperty = AvaloniaProperty.RegisterDirect<CardMetricsView, bool>(
        nameof(ShowRanks),
        o => o.ShowRanks,
        (o, v) => o.ShowRanks = v
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

    public bool ShowRanks
    {
        get => showRanks;
        set => SetAndRaise(ShowRanksProperty, ref showRanks, value);
    }

    private bool highlightImportance = false;
    private bool showRanks = false;
}