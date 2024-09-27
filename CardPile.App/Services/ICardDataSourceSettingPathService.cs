namespace CardPile.App.Services;

internal interface ICardDataSourceSettingPathService : ICardDataSourceSettingService
{
    public string TemporaryValue { get; set; }

    public string Value { get; }
}
