﻿using CardPile.CardData.Settings;

namespace CardPile.App.Services;

internal interface ICardDataSourceSettingService
{
    public string Name { get; }

    public SettingType Type { get; }

    public void ApplyChanges();

    public void DiscardChanges();
}
