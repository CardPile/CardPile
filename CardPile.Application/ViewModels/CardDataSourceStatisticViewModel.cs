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
        Dictionary<string, Bitmap> delimiters = new(){{"{W}", Scryfall.WhiteManaSymbol}, 
                                                      {"{U}", Scryfall.BlueManaSymbol},
                                                      {"{B}", Scryfall.BlackManaSymbol},
                                                      {"{R}", Scryfall.RedManaSymbol},
                                                      {"{G}", Scryfall.GreenManaSymbol},
                                                      {"{C}", Scryfall.ColorlessManaSymbol},
        };
        
        var pattern = $@"(?<={string.Join("|", delimiters.Keys)})";
        var parts = Regex.Split(stat?.cardDataSourceStatisticService.Name ?? "", pattern);
        
        InlineCollection result = new InlineCollection();
        foreach(var part in parts)
        {
            if (delimiters.TryGetValue(part, out var replacement))
            {
                result.Add(new Image { Source = replacement, Width = MANA_SYMBOL_SIZE, Height = MANA_SYMBOL_SIZE });
            }
            else
            {
                result.Add(new Run { Text = part });
            }
        }
        result.Add(new Run() { Text = stat != null ? $": {stat.cardDataSourceStatisticService.TextValue}" : ""});
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
