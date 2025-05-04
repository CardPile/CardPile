using Avalonia.ReactiveUI;
using CardPile.Application.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace CardPile.Application.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        this.WhenActivated(action =>
        {
            action(ViewModel!.ShowCardDataSourceSettingsDialog.RegisterHandler(DoShowCardDataSourceSettingsDialogAsync));
            action(ViewModel!.ShowSettingsDialog.RegisterHandler(DoShowSettingsDialogAsync));
        });
    }

    private async Task DoShowCardDataSourceSettingsDialogAsync(IInteractionContext<CardDataSourceSettingsDialogViewModel, bool> interaction)
    {
        var dialog = new CardDataSourceSettingsWindow
        {
            DataContext = interaction.Input
        };

        interaction.SetOutput(await dialog.ShowDialog<bool>(this));
    }

    private async Task DoShowSettingsDialogAsync(IInteractionContext<SettingsDialogViewModel, bool> interaction)
    {
        var dialog = new SettingsDialog
        {
            DataContext = interaction.Input
        };

        interaction.SetOutput(await dialog.ShowDialog<bool>(this));
    }
}