using CardPile.App.Services;
using CardPile.App.ViewModels;
using CardPile.CardData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardPile.App.Models;

internal class CardDataSourceBuilderModel : ReactiveObject, ICardDataSourceBuilderService
{
    internal CardDataSourceBuilderModel(ICardDataSourceBuilder builder)
    { 
        this.builder = builder;

        settings = builder.Settings.Select(p => p.Type switch
            { 
                CardDataSourceSettingType.Path => new CardDataSourceSettingPathModel((p as ICardDataSourceSettingPath)!),
                _ => new CardDataSourceSettingModel(p)
            }).ToList<ICardDataSourceSettingService>();

        parameters = builder.Parameters.Select(p => p.Type switch
            {
                CardDataSourceParameterType.Options => new CardDataSourceParameterOptionsModel((p as ICardDataSourceParameterOptions)!),
                CardDataSourceParameterType.Date => new CardDataSourceParameterDateModel((p as ICardDataSourceParameterDate)!),
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
