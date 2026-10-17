using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace CardPile.Application.ViewModels;

public class SettingsDialogViewModel : ViewModelBase
{
    internal SettingsDialogViewModel()
    {
        cryptLocation = Configuration.Instance.CryptLocation;
        showAllSkeletons = Configuration.Instance.ShowAllSkeletons;
        synergiesLocation = Configuration.Instance.SynergiesLocation;

        ApplySettingsCommand = ReactiveCommand.Create(() =>
        {
            Configuration.Instance.CryptLocation = CryptLocation;
            Configuration.Instance.ShowAllSkeletons = ShowAllSkeletons;
            Configuration.Instance.SynergiesLocation = SynergiesLocation;
            return true;
        });

        DiscardSettingsCommand = ReactiveCommand.Create(() =>
        {
            return false;
        });

        BrowseFolderInteraction = new Interaction<string, string?>();

        BrowseCryptCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await BrowseFolderInteraction.Handle(CryptLocation);
            if (result != null)
            {
                CryptLocation = result;
            }
        });

        BrowseSynergiesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await BrowseFolderInteraction.Handle(SynergiesLocation);
            if (result != null)
            {
                SynergiesLocation = result;
            }
        });
    }

    internal string CryptLocation
    { 
        get => cryptLocation; 
        set => this.RaiseAndSetIfChanged(ref cryptLocation, value); 
    }

    internal bool ShowAllSkeletons
    {
        get => showAllSkeletons;
        set => this.RaiseAndSetIfChanged(ref showAllSkeletons, value);
    }

    internal string SynergiesLocation
    {
        get => synergiesLocation;
        set => this.RaiseAndSetIfChanged(ref synergiesLocation, value);
    }

    internal ReactiveCommand<Unit, bool> ApplySettingsCommand { get; init; }

    internal ReactiveCommand<Unit, bool> DiscardSettingsCommand { get; init; }

    internal ReactiveCommand<Unit, Unit> BrowseCryptCommand { get; init; }

    internal ReactiveCommand<Unit, Unit> BrowseSynergiesCommand { get; init; }

    internal Interaction<string, string?> BrowseFolderInteraction { get; init; }

    private string cryptLocation;
    private bool showAllSkeletons;
    private string synergiesLocation;
}
