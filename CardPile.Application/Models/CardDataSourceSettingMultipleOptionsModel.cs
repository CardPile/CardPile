using CardPile.Application.Services;
using CardPile.CardData;
using System.Collections.Generic;

namespace CardPile.Application.Models;

internal class CardDataSourceSettingMultipleOptionsModel : CardDataSourceSettingModel, ICardDataSourceSettingMultipleOptionsService
{
    internal CardDataSourceSettingMultipleOptionsModel(ICardDataSourceSettingMultipleOptions setting) : base(setting)
    {
        foreach(var option in As<ICardDataSourceSettingMultipleOptions>()!.Options)
        {
            Options.Add(new CardDataSourceSettingOptionModel(option));
        }
    }

    public override void ApplyChanges()
    {
        foreach (var option in Options)
        {
            option.ApplyChanges();
        }
    }

    public override void DiscardChanges()
    {
        foreach (var option in Options)
        {
            option.DiscardChanges();
        }
    }

    public List<ICardDataSourceSettingOptionService> Options { get; init; } = [];
}

