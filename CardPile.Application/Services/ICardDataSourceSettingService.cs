using CardPile.CardData.Settings;
using ReactiveUI;

namespace CardPile.Application.Services;

internal interface ICardDataSourceSettingService : IReactiveObject
{
    public string Name { get; }

    public SettingType Type { get; }

    public void ApplyChanges();

    public void DiscardChanges();
}
