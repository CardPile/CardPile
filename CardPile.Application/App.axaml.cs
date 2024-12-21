using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CardPile.Application.ViewModels;
using CardPile.Application.Views;
using CardPile.Application.Models;
using System.Threading.Tasks;

namespace CardPile.Application;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splashScreenViewModel = new SplashWindowViewModel();
            var splashScreen = new SplashWindow
            {
                DataContext = splashScreenViewModel
            };

            desktop.MainWindow = splashScreen;
            splashScreen.Show();

            bool shouldContinue;
            try
            {
                await CardPileModel.Init((message) => splashScreenViewModel.StartupMessage = message, splashScreenViewModel.CancellationToken);
                shouldContinue = !splashScreenViewModel.CancellationToken.IsCancellationRequested;
            }
            catch(TaskCanceledException)
            {
                shouldContinue = false;
            }

            if(!shouldContinue)
            {
                splashScreen.Close();
                return;
            }

            var model = new CardPileModel();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(model),
            };
            desktop.MainWindow.Closed += MainWindowClosedHandler;

            desktop.MainWindow.Show();
            splashScreen.Close();
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