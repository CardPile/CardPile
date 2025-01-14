using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class Configuration
{
    public static Configuration Instance { get => instance.Value; }

    private Configuration()
    { }

    [JsonProperty("location")]
    public string Location { get; set; } = Path.Combine(Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".", "Grades.csv");

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

    private static readonly string CONFIG_FILENAME = Path.Combine(Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".", "spreadsheet.config.json");

    private static Lazy<Configuration> instance = new Lazy<Configuration>(() => Load());
}
