﻿using CardPile.Application.Services;
using CardPile.CardData;
using CardPile.CardData.Parameters;
using CardPile.CardData.Settings;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardPile.Application.Models;

internal class CardDataSourceBuilderModel : ReactiveObject, ICardDataSourceBuilderService
{
    internal CardDataSourceBuilderModel(ICardDataSourceBuilder builder)
    { 
        this.builder = builder;

        settings = builder.Settings.Select(p => p.Type switch
            { 
                SettingType.Path => new CardDataSourceSettingPathModel((p as ICardDataSourceSettingPath)!),
                SettingType.Number => new CardDataSourceSettingNumberModel((p as ICardDataSourceSettingNumber)!),
                SettingType.Decimal => new CardDataSourceSettingDecimalModel((p as ICardDataSourceSettingDecimal)!),
                SettingType.Option => new CardDataSourceSettingOptionModel((p as ICardDataSourceSettingOption)!),
                SettingType.MultipleOptions => new CardDataSourceSettingMultipleOptionsModel((p as ICardDataSourceSettingMultipleOptions)!),
                _ => new CardDataSourceSettingModel(p)
            }).ToList<ICardDataSourceSettingService>();

        parameters = builder.Parameters.Select(p => p.Type switch
            {
                ParameterType.Options => new CardDataSourceParameterOptionsModel((p as ICardDataSourceParameterOptions)!),
                ParameterType.Date => new CardDataSourceParameterDateModel((p as ICardDataSourceParameterDate)!),
                _ => new CardDataSourceParameterModel(p)
            }).ToList<ICardDataSourceParameterService>();

        metricDescriptions = builder.MetricDescriptions.Select(p => new CardMetricDescriptionModel(p))
                                                       .ToList<ICardMetricDescriptionService>();
    }

    public string Name
    {
        get => builder.Name;
    }

    public List<ICardDataSourceSettingService> Settings
    {
        get => settings;
    }

    public List<ICardDataSourceParameterService> Parameters
    {
        get => parameters;
    }

    public List<ICardMetricDescriptionService> MetricDescriptions
    {
        get => metricDescriptions;
    }

    public ICardDataSource BuildDataSource()
    {
        return builder.Build();
    }

    public Task<ICardDataSource> BuildDataSourceAsync(CancellationToken cancellation)
    {
        return builder.BuildAsync(cancellation);
    }

    private readonly ICardDataSourceBuilder builder;
    private readonly List<ICardDataSourceSettingService> settings;
    private readonly List<ICardDataSourceParameterService> parameters;
    private readonly List<ICardMetricDescriptionService> metricDescriptions;
}
