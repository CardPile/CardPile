using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class Configuration
{
    public static Configuration Instance { get => instance.Value; }

    private Configuration()
    {}

    [JsonProperty("rank_colors_to_show")]
    public List<string> RankColorsToShow { get; set; } = [CardDataSourceBuilder.ONE_COLOR_RANK_COLOR_OPTION_NAME, CardDataSourceBuilder.TWO_COLOR_RANK_COLOR_OPTION_NAME];

    [JsonProperty("max_rank_to_show")]
    public int MaxRankToShow { get; set; } = DEFAULT_MAX_RANK_TO_SHOW;

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

    private static readonly string CONFIG_FILENAME = Path.Combine(Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".", "seventeenlands.config.json");

    private static Lazy<Configuration> instance = new Lazy<Configuration>(() => Load());

    private const int DEFAULT_MAX_RANK_TO_SHOW = 20;
}
