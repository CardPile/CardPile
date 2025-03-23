using System.Collections.Generic;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using CardPile.Application.Services;
using CardPile.CardInfo;

namespace CardPile.Application.ViewModels;

public class CardDataSourceStatisticViewModel : ViewModelBase
{
    private const int MANA_SYMBOL_SIZE = 13;
    
    public static FuncValueConverter<CardDataSourceStatisticViewModel, InlineCollection> StatisticToInlinesConverter { get; } = new FuncValueConverter<CardDataSourceStatisticViewModel, InlineCollection>(stat =>
    {
        var result = ConverterUtils.TextToInlines(stat?.cardDataSourceStatisticService.Name);
        result.Add(new Run() { Text = stat != null ? $": {stat.cardDataSourceStatisticService.TextValue}" : "" });
        return result;
    });
    
    internal CardDataSourceStatisticViewModel(ICardDataSourceStatisticService cardDataSourceStatisticService)
    {
        this.cardDataSourceStatisticService = cardDataSourceStatisticService;
    }

    internal ICardDataSourceStatisticService CardDataSourceStatisticService
    {
        get => cardDataSourceStatisticService;
    }

    private readonly ICardDataSourceStatisticService cardDataSourceStatisticService;
}
