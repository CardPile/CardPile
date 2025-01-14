using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;

namespace CardPile.Application.Models;

internal class CardDataSourceSettingOptionModel : CardDataSourceSettingModel, ICardDataSourceSettingOptionService
{
    internal CardDataSourceSettingOptionModel(ICardDataSourceSettingOption setting) : base(setting)
    {
        temporaryValue = As<ICardDataSourceSettingOption>()!.Value;
    }

    public override void ApplyChanges()
    {
        if (As<ICardDataSourceSettingOption>()!.Value != temporaryValue)
        {
            this.RaisePropertyChanging(nameof(Value));
            As<ICardDataSourceSettingOption>()!.Value = temporaryValue;
            this.RaisePropertyChanged(nameof(Value));
        }
    }

    public override void DiscardChanges()
    {
        temporaryValue = As<ICardDataSourceSettingOption>()!.Value;
    }

    public bool TemporaryValue
    {
        get => temporaryValue;
        set => this.RaiseAndSetIfChanged(ref temporaryValue, value);
    }

    public bool Value
    {
        get => As<ICardDataSourceSettingOption>()!.Value;
    }

    public bool temporaryValue;
}

