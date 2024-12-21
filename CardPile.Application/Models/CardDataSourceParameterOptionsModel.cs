using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;

namespace CardPile.Application.Models;

internal class CardDataSourceParameterOptionsModel : CardDataSourceParameterModel, ICardDataSourceParameterOptionsService
{
    internal CardDataSourceParameterOptionsModel(ICardDataSourceParameterOptions parameter) : base(parameter)
    {
    }

    public List<string> Options
    { 
        get => As<ICardDataSourceParameterOptions>()!.Options; 
    }

    public string Value
    { 
        get => As<ICardDataSourceParameterOptions>()!.Value;
        set
        {
            if (As<ICardDataSourceParameterOptions>()!.Value != value)
            {
                this.RaisePropertyChanging(nameof(Value));
                As<ICardDataSourceParameterOptions>()!.Value = value;
                this.RaisePropertyChanged(nameof(Value));
            } 
        }
    }
}
