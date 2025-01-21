using CardPile.Application.Services;
using CardPile.CardData;
using ReactiveUI;
using System;

namespace CardPile.Application.Models;

internal class CardDataSourceParameterDateModel : CardDataSourceParameterModel, ICardDataSourceParameterDateService
{
    internal CardDataSourceParameterDateModel(ICardDataSourceParameterDate parameter) : base(parameter)
    {
        parameter.PropertyChanging += OnParameterPropertyChanging;
        parameter.PropertyChanged += OnParameterPropertyChanged;
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

    private void OnParameterPropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
    {
        this.RaisePropertyChanging(nameof(Value));
    }

    private void OnParameterPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(Value));
    }
}
