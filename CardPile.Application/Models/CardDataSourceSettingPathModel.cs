using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;

namespace CardPile.Application.Models;

internal class CardDataSourceSettingPathModel : CardDataSourceSettingModel, ICardDataSourceSettingPathService
{
    internal CardDataSourceSettingPathModel(ICardDataSourceSettingPath setting) : base(setting)
    {
        temporaryValue = As<ICardDataSourceSettingPath>()!.Value;
    }

    public override void ApplyChanges()
    {
        if (As<ICardDataSourceSettingPath>()!.Value != temporaryValue)
        {
            this.RaisePropertyChanging(nameof(Value));
            As<ICardDataSourceSettingPath>()!.Value = temporaryValue;
            this.RaisePropertyChanged(nameof(Value));
        }
    }

    public override void DiscardChanges()
    {
        temporaryValue = As<ICardDataSourceSettingPath>()!.Value;
    }

    public string TemporaryValue
    {
        get => temporaryValue;
        set => this.RaiseAndSetIfChanged(ref temporaryValue, value);
    }

    public string Value
    {
        get => As<ICardDataSourceSettingPath>()!.Value;
    }

    public string temporaryValue;
}

