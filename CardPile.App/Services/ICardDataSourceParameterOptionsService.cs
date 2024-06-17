using System.Collections.Generic;

namespace CardPile.App.Services;

internal interface ICardDataSourceParameterOptionsService : ICardDataSourceParameterService
{
    public List<string> Options { get; }

    public string Value { get; set; }
}
