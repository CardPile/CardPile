using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CardPile.CardData.Parameters;

public class Parameter : ICardDataSourceParameter
{
    protected Parameter(string name, ParameterType type)
    {
        Name = name;
        Type = type;
    }

    public event PropertyChangingEventHandler? PropertyChanging;
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; init; }

    public ParameterType Type { get; init; }

    protected void RaiseAndSetIfChanged<TRet>(ref TRet backingField, TRet newValue, [CallerMemberName] string? propertyName = null)
    {
        if(string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
        {
            return;
        }

        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        backingField = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
