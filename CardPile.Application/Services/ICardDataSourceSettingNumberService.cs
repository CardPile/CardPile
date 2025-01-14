namespace CardPile.Application.Services;

internal interface ICardDataSourceSettingNumberService : ICardDataSourceSettingService
{
    public int TemporaryValue { get; set; }

    public int Value { get; }

    public int MinAllowedValue { get; }

    public int MaxAllowedValue { get; }
}
