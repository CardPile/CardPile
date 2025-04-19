using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;

namespace CardPile.Application.Models;

internal class CardDataSourceSettingDecimalModel : CardDataSourceSettingModel, ICardDataSourceSettingDecimalService
{
    internal CardDataSourceSettingDecimalModel(ICardDataSourceSettingDecimal setting) : base(setting)
    {
        temporaryValue = As<ICardDataSourceSettingDecimal>()!.Value;
    }

    public override void ApplyChanges()
    {
        if (As<ICardDataSourceSettingDecimal>()!.Value != temporaryValue)
        {
            this.RaisePropertyChanging(nameof(Value));
            As<ICardDataSourceSettingDecimal>()!.Value = temporaryValue;
            this.RaisePropertyChanged(nameof(Value));
        }
    }

    public override void DiscardChanges()
    {
        temporaryValue = As<ICardDataSourceSettingDecimal>()!.Value;
    }

    public decimal TemporaryValue
    {
        get => temporaryValue;
        set => this.RaiseAndSetIfChanged(ref temporaryValue, value);
    }

    public decimal Value
    {
        get => As<ICardDataSourceSettingDecimal>()!.Value;
    }

    public decimal MinAllowedValue
    {
        get => As<ICardDataSourceSettingDecimal>()!.MinAllowedValue;
    }

    public decimal MaxAllowedValue
    {
        get => As<ICardDataSourceSettingDecimal>()!.MaxAllowedValue;
    }

    public decimal Increment
    {
        get => As<ICardDataSourceSettingDecimal>()!.Increment;
    }
    
    public decimal temporaryValue;
}

