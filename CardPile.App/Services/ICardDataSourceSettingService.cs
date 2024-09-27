using CardPile.CardData;

namespace CardPile.App.Services;

internal interface ICardDataSourceSettingService
{
    public string Name { get; }

    public CardDataSourceSettingType Type { get; }

    public void ApplyChanges();

    public void DiscardChanges();
}
