using CardPile.App.Services;
using CardPile.CardData;
using ReactiveUI;
using System;

namespace CardPile.App.Models;

internal class CardDataSourceParameterDateModel : CardDataSourceParameterModel, ICardDataSourceParameterDateService
{
    internal CardDataSourceParameterDateModel(ICardDataSourceParameterDate parameter) : base(parameter)
    {
    }

    public DateTime Value
    {
        get => As<ICardDataSourceParameterDate>()!.Value;
        set
        {
            if (As<ICardDataSourceParameterDate>()!.Value != value)
            {
                this.RaisePropertyChanging(nameof(Value));
                As<ICardDataSourceParameterDate>()!.Value = value;
                this.RaisePropertyChanged(nameof(Value));
            }
        }
    }
}
