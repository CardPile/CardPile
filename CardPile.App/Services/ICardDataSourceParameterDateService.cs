using System;

namespace CardPile.App.Services;

internal interface ICardDataSourceParameterDateService : ICardDataSourceParameterService
{
    public DateTime Value { get; set; }
}
