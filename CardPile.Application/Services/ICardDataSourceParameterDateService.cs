using System;

namespace CardPile.Application.Services;

internal interface ICardDataSourceParameterDateService : ICardDataSourceParameterService
{
    public DateTime Value { get; set; }
}
