using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using CardPile.Application.Services;

namespace CardPile.Application.ViewModels;

public class CardDataSourceStatisticViewModel : ViewModelBase
{
   
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
