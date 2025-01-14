using System.Collections.Generic;

namespace CardPile.Application.Services;

internal interface ICardDataSourceSettingMultipleOptionsService : ICardDataSourceSettingService
{
    public List<ICardDataSourceSettingOptionService> Options { get; }
}
