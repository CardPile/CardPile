namespace CardPile.Application.Services;

internal interface ICardDataSourceSettingOptionService : ICardDataSourceSettingService
{
    public bool TemporaryValue { get; set; }

    public bool Value { get; }
}
