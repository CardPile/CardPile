using Newtonsoft.Json;

namespace CardPile.CardData.SeventeenLands;

internal class Configuration
{
    public static Configuration Instance { get => LazyInstance.Value; }

    private Configuration()
    {}

    [JsonProperty("current_set_start_date_offset_in_days")]
    public int CurrentSetStartDateOffsetInDays { get; set; } = DefaultCurrentSetStartDateOffsetInDays;

    [JsonProperty("win_rate_colors_to_show")]
    public List<string> WinRateColorsToShow { get; set; } = [CardDataSourceBuilder.ONE_COLOR_RANK_COLOR_OPTION_NAME, CardDataSourceBuilder.TWO_COLOR_RANK_COLOR_OPTION_NAME, CardDataSourceBuilder.THREE_COLOR_RANK_COLOR_OPTION_NAME];

    [JsonProperty("win_rate_participation_cutoff")]
    public decimal WinRateParticipationCutoff { get; set; } = DefaultWinRateParticipationCutoff;

    [JsonProperty("rank_colors_to_show")]
    public List<string> RankColorsToShow { get; set; } = [CardDataSourceBuilder.ONE_COLOR_RANK_COLOR_OPTION_NAME, CardDataSourceBuilder.TWO_COLOR_RANK_COLOR_OPTION_NAME];

    [JsonProperty("max_rank_to_show")]
    public int MaxRankToShow { get; set; } = DefaultMaxRankToShow;

    [JsonProperty("deq_damping_sample")]
    public int DEqDampingSample { get; set; } = DefaultDeqDampingSample;

    [JsonProperty("deq_ata_beta")]
    public decimal DEqAtaBeta { get; set; } = DefaultDeqAtaBeta;

    [JsonProperty("deq_p1p1_value")]
    public decimal DEqP1P1Value { get; set; } = DefaultDeqP1P1Value;

    [JsonProperty("deq_archetype_decay")]
    public decimal DEqArchetypeDecay { get; set; } = DefaultDeqArchetypeDecay;

    [JsonProperty("deq_loss_factor")]
    public decimal DEqLossFactor { get; set; } = DefaultDeqLossFactor;

    [JsonProperty("deq_sample_decay")]
    public decimal DEqSampleDecay { get; set; } = DefaultDeqSampleDecay;

    [JsonProperty("deq_future_projection_days")]
    public int DEqFutureProjectionDays { get; set; } = DefaultDeqFutureProjectionDays;

    [JsonProperty("deq_colors")]
    public List<string> DEqColors { get; set; } = [CardDataSourceBuilder.ONE_COLOR_RANK_COLOR_OPTION_NAME, CardDataSourceBuilder.TWO_COLOR_RANK_COLOR_OPTION_NAME];

    public async Task Save()
    {
        string json = JsonConvert.SerializeObject(this);
        await File.WriteAllTextAsync(ConfigFilename, json);
    }

    private static Configuration Load()
    {
        if (!File.Exists(ConfigFilename))
        {
            return new Configuration();
        }

        var jsonText = File.ReadAllText(ConfigFilename);
        var deserializedConfig = JsonConvert.DeserializeObject<Configuration>(jsonText, new JsonSerializerSettings()
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        });
        
        return deserializedConfig ?? new Configuration();
    }

    private static readonly Lazy<Configuration> LazyInstance = new(Load);
    
    private static readonly string ConfigFilename = Path.Combine(Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".", "seventeenlands.config.json");
    
    private const decimal DefaultWinRateParticipationCutoff = 1.0M;

    private const int DefaultMaxRankToShow = 20;

    private const int DefaultCurrentSetStartDateOffsetInDays = 14;

    private const int DefaultDeqDampingSample = 25;
    private const decimal DefaultDeqAtaBeta = -0.0033m;
    private const decimal DefaultDeqP1P1Value = 0.025m;
    private const decimal DefaultDeqArchetypeDecay = 0.95m;
    private const decimal DefaultDeqLossFactor = 0.6m;
    private const decimal DefaultDeqSampleDecay = 0.95m;
    private const int DefaultDeqFutureProjectionDays = 0;
}
