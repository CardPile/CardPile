using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;
using System.Collections.Generic;

namespace CardPile.Application.Models;

internal class CardDataSourceParameterOptionsModel : CardDataSourceParameterModel, ICardDataSourceParameterOptionsService
{
    internal CardDataSourceParameterOptionsModel(ICardDataSourceParameterOptions parameter) : base(parameter)
    {
        parameter.PropertyChanging += OnParameterPropertyChanging;
        parameter.PropertyChanged += OnParameterPropertyChanged;
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

    private void OnParameterPropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
    {
        this.RaisePropertyChanging(nameof(Value));
    }

    private void OnParameterPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(Value));
    }
}
