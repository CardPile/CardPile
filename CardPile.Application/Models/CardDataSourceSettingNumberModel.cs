using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;

namespace CardPile.Application.Models;

internal class CardDataSourceSettingNumberModel : CardDataSourceSettingModel, ICardDataSourceSettingNumberService
{
    internal CardDataSourceSettingNumberModel(ICardDataSourceSettingNumber setting) : base(setting)
    {
        temporaryValue = As<ICardDataSourceSettingNumber>()!.Value;
    }

    public override void ApplyChanges()
    {
        if (As<ICardDataSourceSettingNumber>()!.Value != temporaryValue)
        {
            this.RaisePropertyChanging(nameof(Value));
            As<ICardDataSourceSettingNumber>()!.Value = temporaryValue;
            this.RaisePropertyChanged(nameof(Value));
        }
    }

    public override void DiscardChanges()
    {
        temporaryValue = As<ICardDataSourceSettingNumber>()!.Value;
    }

    public int TemporaryValue
    {
        get => temporaryValue;
        set => this.RaiseAndSetIfChanged(ref temporaryValue, value);
    }

    public int Value
    {
        get => As<ICardDataSourceSettingNumber>()!.Value;
    }

    public int MinAllowedValue
    { 
        get => As<ICardDataSourceSettingNumber>()!.MinAllowedValue;
    }

    public int MaxAllowedValue 
    {
        get => As<ICardDataSourceSettingNumber>()!.MaxAllowedValue;
    }

    public int temporaryValue;
}

