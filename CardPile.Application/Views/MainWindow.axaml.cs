using Avalonia.ReactiveUI;
using CardPile.Application.Services;
using CardPile.Application.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardPile.Application.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        this.WhenActivated(action => action(ViewModel!.ShowCardDataSourceSettingsDialog.RegisterHandler(DoShowCardDataSourceSettingsDialogAsync)));
    }

    private async Task DoShowCardDataSourceSettingsDialogAsync(IInteractionContext<CardDataSourceSettingsDialogViewModel, bool> interaction)
    {
        var dialog = new CardDataSourceSettingsWindow
        {
            DataContext = interaction.Input
        };

        interaction.SetOutput(await dialog.ShowDialog<bool>(this));
    }

}