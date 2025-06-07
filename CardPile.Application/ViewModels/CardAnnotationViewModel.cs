using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using CardPile.Application.Services;

namespace CardPile.Application.ViewModels;

internal class CardAnnotationViewModel : ViewModelBase
{
    public static FuncValueConverter<string, InlineCollection> AnnotationTextToInlinesConverter { get; } = new FuncValueConverter<string, InlineCollection>(text => ConverterUtils.TextToInlines(text));

    internal CardAnnotationViewModel(ICardAnnotationService cardAnnotation)
    {
        this.cardAnnotation = cardAnnotation;
    }

    internal string Name
    {
        get => cardAnnotation.Name;
    }

    internal string TextValue
    {
        get => cardAnnotation.TextValue;
    }

    private ICardAnnotationService cardAnnotation;
}
