using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CardPile.Application;

internal class Configuration : INotifyPropertyChanged
{
    public static Configuration Instance { get => instance.Value; }

    private Configuration()
    { }

    [JsonProperty("crypt_location")]
    public string CryptLocation
    { 
        get => cryptLocation; 
        set => RaiseAndSetIfChanged(ref cryptLocation, value); 
    } 

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task Save()
    {
        string json = JsonConvert.SerializeObject(this);
        await File.WriteAllTextAsync(CONFIG_FILENAME, json);
    }

    private static Configuration Load()
    {
        if (!File.Exists(CONFIG_FILENAME))
        {
            return new Configuration();
        }

        var jsonText = File.ReadAllText(CONFIG_FILENAME);
        return JsonConvert.DeserializeObject<Configuration>(jsonText) ?? new Configuration();
    }

    private TRet RaiseAndSetIfChanged<TRet>(ref TRet backingField, TRet newValue, [CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
        {
            return newValue;
        }

        backingField = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return newValue;
    }

    private static readonly string CONFIG_FILENAME = Path.Combine(Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".", "config.json");

    private static Lazy<Configuration> instance = new Lazy<Configuration>(() => Load());

    private string cryptLocation = Path.Combine(Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".", "Skeletons");
}
