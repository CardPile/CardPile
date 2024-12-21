using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.CardData.Settings;
using ReactiveUI;

namespace CardPile.Application.Models;

internal class CardDataSourceSettingModel : ReactiveObject, ICardDataSourceSettingService
{
    internal CardDataSourceSettingModel(ICardDataSourceSetting setting)
    {
        this.setting = setting;
    }

    public string Name { get => setting.Name; }

    public SettingType Type { get => setting.Type; }

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
