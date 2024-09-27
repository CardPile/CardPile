using CardPile.App.Services;
using CardPile.CardData;
using ReactiveUI;

namespace CardPile.App.Models;

internal class CardDataSourceSettingModel : ReactiveObject, ICardDataSourceSettingService
{
    internal CardDataSourceSettingModel(ICardDataSourceSetting setting)
    {
        this.setting = setting;
    }

    public string Name { get => setting.Name; }

    public CardDataSourceSettingType Type { get => setting.Type; }

    virtual public void ApplyChanges()
    { }

    virtual public void DiscardChanges()
    { }

    protected T? As<T>() where T : class, ICardDataSourceSetting
    {
        return setting as T;
    }

    private ICardDataSourceSetting setting;
}
