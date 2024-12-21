using CardPile.Application.Services;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace CardPile.Application.ViewModels;

public class CardDataSourceSettingsDialogViewModel : ViewModelBase
{
    internal CardDataSourceSettingsDialogViewModel(List<ICardDataSourceSettingService> settings)
    {
        this.settings = settings;

        ApplySettingsCommand = ReactiveCommand.Create(() =>
        {
            return true;
        });

        DiscardSettingsCommand = ReactiveCommand.Create(() =>
        {
            return false;
        });

        BrowseFileInteraction = new Interaction<string, string?>();

        BrowseFileCommand = ReactiveCommand.CreateFromTask(async (ICardDataSourceSettingPathService service) =>
        {
            var result = await BrowseFileInteraction.Handle(service.Value);
            if (result != null)
            {
                service.TemporaryValue = result;
            }
        });
    }

    internal List<ICardDataSourceSettingService> Settings { get => settings; }

    internal void ApplyChanges()
    {
        foreach (var setting in settings)
        {
            setting.ApplyChanges();
        }
    }

    internal void DiscardChanges()
    {
        foreach (var setting in settings)
        {
            setting.DiscardChanges();
        }
    }

    internal ReactiveCommand<Unit, bool> ApplySettingsCommand { get; init; }

    internal ReactiveCommand<Unit, bool> DiscardSettingsCommand { get; init; }

    internal Interaction<string, string?> BrowseFileInteraction { get; init; }

    internal ReactiveCommand<ICardDataSourceSettingPathService, Unit> BrowseFileCommand { get; init; }

    private List<ICardDataSourceSettingService> settings;
}
