using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data;

namespace CardPile.App.Extensions;

public abstract class TextBlockExtensions : AvaloniaObject
{
    static TextBlockExtensions()
    {
        BindableInlinesProperty.Changed.AddClassHandler<TextBlock>(HandleBindableInlinesChanged);
    }
    
    public static readonly AttachedProperty<IEnumerable<Inline>> BindableInlinesProperty =
        AvaloniaProperty.RegisterAttached<TextBlockExtensions, TextBlock, IEnumerable<Inline>>(
        "BindableInlines", 
        [], 
        false, 
        BindingMode.OneTime
    );
    
    private static void HandleBindableInlinesChanged(TextBlock block, AvaloniaPropertyChangedEventArgs args)
    {
        block.Inlines?.Clear();
        if (args.NewValue is IEnumerable<Inline> inlinesValue)
        {
            block.Inlines?.AddRange(inlinesValue);
        }
    }
    
    public static void SetBindableInlines(AvaloniaObject element, IEnumerable<Inline> commandValue)
    {
        element.SetValue(BindableInlinesProperty, commandValue);
    }
    
    public static IEnumerable<Inline> GetBindableInlines(AvaloniaObject element)
    {
        return element.GetValue(BindableInlinesProperty);
    }
}