using Avalonia.Controls.Documents;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using CardPile.CardInfo;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CardPile.CardData.Importance;
using System;

namespace CardPile.Application.ViewModels;

internal class ConverterUtils
{
    public static string HIGHLIGHT_GREEN_MARKER = "{HighlightGreen}";

    public static IBrush ImportanceToBrush(ImportanceLevel level)
    {
        switch (level)
        {
            case ImportanceLevel.Low: return new SolidColorBrush(Colors.LightGray);
            case ImportanceLevel.Regular:
                {
                    if (!Avalonia.Application.Current!.TryGetResource("SystemControlForegroundBaseHighBrush", Avalonia.Application.Current.ActualThemeVariant, out object? foregroundBrush))
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                    var result = foregroundBrush as IBrush;
                    return result ?? new SolidColorBrush(Colors.White);
                }
            case ImportanceLevel.High: return new SolidColorBrush(Colors.Orange);
            case ImportanceLevel.Critical: return new SolidColorBrush(Colors.Red);
            default: throw new System.NotImplementedException();
        }
    }

    private class TextToInlinesState
    {
        public IBrush? Foreground = null;
        public string Text = string.Empty;
    };

    public static InlineCollection TextToInlines(string? text)
    {
        const int MANA_SYMBOL_SIZE = 13;

        string IMPORTANCE_LOW = $"{{ImportanceLevel.{ImportanceLevel.Low}}}";
        string IMPORTANCE_REGULAR = $"{{ImportanceLevel.{ImportanceLevel.Regular}}}";
        string IMPORTANCE_HIGH = $"{{ImportanceLevel.{ImportanceLevel.High}}}";
        string IMPORTANCE_CRITICAL = $"{{ImportanceLevel.{ImportanceLevel.Critical}}}";

        Func<Bitmap, Control?> manaSymbolReplacer = (Bitmap replacement) =>
        {
            var image = new Image { Source = replacement, Width = MANA_SYMBOL_SIZE, Height = MANA_SYMBOL_SIZE };
            RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.HighQuality);
            return image;
        };

        Func<TextToInlinesState, ImportanceLevel, Control?> importanceReplacer = (TextToInlinesState state, ImportanceLevel level) =>
        {
            state.Foreground = ImportanceToBrush(level);
            return null;
        };

        Func<TextToInlinesState, Color, Control?> highlightReplacer = (TextToInlinesState state, Color color) =>
        {
            state.Foreground = new SolidColorBrush(color);
            return null;
        };

        Dictionary<string, Func<TextToInlinesState, Control?>> delimiters = new(){{"{W}", _ => manaSymbolReplacer(Scryfall.WhiteManaSymbol) },
                                                                                  {"{U}", _ => manaSymbolReplacer(Scryfall.BlueManaSymbol) },
                                                                                  {"{B}", _ => manaSymbolReplacer(Scryfall.BlackManaSymbol) },
                                                                                  {"{R}", _ => manaSymbolReplacer(Scryfall.RedManaSymbol) },
                                                                                  {"{G}", _ => manaSymbolReplacer(Scryfall.GreenManaSymbol) },
                                                                                  {"{C}", _ => manaSymbolReplacer(Scryfall.ColorlessManaSymbol) },
                                                                                  {IMPORTANCE_LOW, s => importanceReplacer(s, ImportanceLevel.Low) },
                                                                                  {IMPORTANCE_REGULAR, s => importanceReplacer(s, ImportanceLevel.Regular) },
                                                                                  {IMPORTANCE_HIGH, s => importanceReplacer(s, ImportanceLevel.High) },
                                                                                  {IMPORTANCE_CRITICAL, s => importanceReplacer(s, ImportanceLevel.Critical) },
                                                                                  {HIGHLIGHT_GREEN_MARKER, s => highlightReplacer(s, Colors.Green)}

        };

        var pattern = $@"({string.Join("|", delimiters.Keys)})";
        var parts = Regex.Split(text ?? "", pattern);

        TextToInlinesState state = new TextToInlinesState();
        InlineCollection result = [];
        foreach (var part in parts)
        {
            state.Text = part;
            if (delimiters.TryGetValue(part, out var replacementFunc))
            {
                var control = replacementFunc(state);
                if (control != null)
                {
                    result.Add(control);
                }
            }
            else if (part.Length > 0)
            {
                var run = new Run { Text = part };
                if(state.Foreground != null)
                {
                    run.Foreground = state.Foreground;
                }
                result.Add(run);
                
            }
        }
        return result;
    }
}
