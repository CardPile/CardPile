using CardPile.Application.Services;
using ReactiveUI;

namespace CardPile.Application.Models
{
    internal class CardAnnotationModel : ReactiveObject, ICardAnnotationService
    {
        internal CardAnnotationModel(string name, string textValue)
        {
            Name = name;
            TextValue = textValue;
        }

        public string Name { get; init; }

        public string TextValue {  get; init; }
    }
}
