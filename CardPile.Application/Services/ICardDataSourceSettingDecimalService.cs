namespace CardPile.Application.Services;

internal interface ICardDataSourceSettingDecimalService : ICardDataSourceSettingService
{
    public decimal TemporaryValue { get; set; }

    public decimal Value { get; }

    public decimal MinAllowedValue { get; }

    public decimal MaxAllowedValue { get; }
}
