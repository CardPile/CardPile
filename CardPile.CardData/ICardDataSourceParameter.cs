using CardPile.CardData.Parameters;
using System.ComponentModel;

namespace CardPile.CardData;

public interface ICardDataSourceParameter : INotifyPropertyChanging, INotifyPropertyChanged
{
    public string Name { get; }

    public ParameterType Type { get; }
}
