using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using CardPile.App.ViewModels;
using ReactiveUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CardPile.App;

public partial class CardDataSourceSettingsWindow : ReactiveWindow<CardDataSourceSettingsDialogViewModel>
{
    public CardDataSourceSettingsWindow()
    {
        InitializeComponent();

        // This line is needed to make the previewer happy (the previewer plugin cannot handle the following line).
        if (Design.IsDesignMode) return;

        this.WhenActivated(action => action(ViewModel!.ApplySettingsCommand.Subscribe(x => Close(x))));
        this.WhenActivated(action => action(ViewModel!.DiscardSettingsCommand.Subscribe(x => Close(x))));
        this.WhenActivated(action => action(ViewModel!.BrowseFileInteraction.RegisterHandler(BrowseFileInteractionHandler)));
    }

    private async Task BrowseFileInteractionHandler(IInteractionContext<string, string?> context)
    {
        var topLevel = GetTopLevel(this);

        var folder = System.IO.Path.GetDirectoryName(context.Input) ?? string.Empty;
        var startFolder = await StorageProvider.TryGetFolderFromPathAsync(folder);
        var selectedFiles = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Select file",
            SuggestedStartLocation = startFolder,
        });

        var selectedFile = selectedFiles.FirstOrDefault();
        if (selectedFile != null)
        {
            context.SetOutput(HttpUtility.UrlDecode(selectedFile.Path.AbsolutePath));
        }
        else
        {
            context.SetOutput(null);
        }
    }
}