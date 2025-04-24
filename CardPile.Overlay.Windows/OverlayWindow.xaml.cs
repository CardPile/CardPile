using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace CardPile.Overlay.Windows;

public partial class OverlayWindow : Window
{
    public OverlayWindow()
    {
        InitializeComponent();

        CompositionTarget.Rendering += Update;
    }

    private void Update(object? sender, EventArgs e)
    {
        Process? mtgaProcess = Process.GetProcessesByName(MTGA_PROCESS_NAME).FirstOrDefault();
        if (mtgaProcess == null)
        {
            App.Current.Shutdown();
            return;
        }

        if (!InteropUtils.GetWindowRect(mtgaProcess.MainWindowHandle, out var rect))
        {
            App.Current.Shutdown();
            return;
        }

        Left = rect.Left;
        Top = rect.Top;
        Width = rect.Right - rect.Left;
        Height = rect.Bottom - rect.Top;
    }

    // private readonly string MTGA_PROCESS_NAME = "MTGA";
    private readonly string MTGA_PROCESS_NAME = "notepad";
}