using Avalonia;
using Avalonia.Controls;

namespace CardPile.Application.Views;

public partial class CardMetricView : UserControl
{
    public static readonly DirectProperty<CardMetricView, bool> HighlightImportanceProperty = AvaloniaProperty.RegisterDirect<CardMetricView, bool>(
        nameof(HighlightImportance),
        o => o.HighlightImportance,
        (o, v) => o.HighlightImportance = v
    );

    public static readonly DirectProperty<CardMetricView, bool> ShowRanksProperty = AvaloniaProperty.RegisterDirect<CardMetricView, bool>(
        nameof(ShowRanks),
        o => o.ShowRanks,
        (o, v) => o.ShowRanks = v
    );

    public static readonly DirectProperty<CardMetricView, bool> ExpandRanksProperty = AvaloniaProperty.RegisterDirect<CardMetricView, bool>(
        nameof(ExpandRanks),
        o => o.ExpandRanks,
        (o, v) => o.ExpandRanks = v
    );

    public CardMetricView()
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

    public bool ExpandRanks
    {
        get => expandRanks;
        set => SetAndRaise(ExpandRanksProperty, ref expandRanks, value);
    }

    public void ExpandRanksCommand()
    {
        ExpandRanks = !ExpandRanks;
    }

    private bool highlightImportance = false;
    private bool showRanks = false;
    private bool expandRanks = false;
}