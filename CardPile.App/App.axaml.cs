using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CardPile.App.ViewModels;
using CardPile.App.Views;
using CardPile.App.Models;

namespace CardPile.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var model = new CardPileModel();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(model),
            };
            desktop.MainWindow.Closed += MainWindowClosedHandler;
        }
    }

    private void MainWindowClosedHandler(object? sender, System.EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}