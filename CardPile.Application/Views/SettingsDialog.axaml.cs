using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using CardPile.Application.ViewModels;
using ReactiveUI;

namespace CardPile.Application.Views;

public partial class SettingsDialog : ReactiveWindow<SettingsDialogViewModel>
{
    public SettingsDialog()
    {
        InitializeComponent();

        // This line is needed to make the previewer happy (the previewer plugin cannot handle the following line).
        if (Design.IsDesignMode) return;

        this.WhenActivated(action => action(ViewModel!.ApplySettingsCommand.Subscribe(x => Close(x))));
        this.WhenActivated(action => action(ViewModel!.DiscardSettingsCommand.Subscribe(x => Close(x))));
        this.WhenActivated(action => action(ViewModel!.BrowseFolderInteraction.RegisterHandler(BrowseFolderInteractionHandler)));
    }

    private async Task BrowseFolderInteractionHandler(IInteractionContext<string, string?> context)
    {
        var topLevel = GetTopLevel(this);

        var folder = System.IO.Path.GetDirectoryName(context.Input) ?? string.Empty;
        var startFolder = await StorageProvider.TryGetFolderFromPathAsync(folder);
        var selectedFolders = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Select folder",
            SuggestedStartLocation = startFolder,
        });

        var selectedFolder = selectedFolders.FirstOrDefault();
        if (selectedFolder != null)
        {
            context.SetOutput(HttpUtility.UrlDecode(selectedFolder.Path.AbsolutePath));
        }
        else
        {
            context.SetOutput(null);
        }
    }
}