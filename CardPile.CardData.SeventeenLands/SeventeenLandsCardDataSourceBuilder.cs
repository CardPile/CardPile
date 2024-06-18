using Newtonsoft.Json;
using System.Text;
using System.Web;

namespace CardPile.CardData.SeventeenLands;

public class SeventeenLandsCardDataSourceBuilder : ICardDataSourceBuilder
{
    static SeventeenLandsCardDataSourceBuilder()
    {
        BuildFilters();
    }

    public SeventeenLandsCardDataSourceBuilder()
    {
        Parameters =
        [
            SetParameter,
            EventTypeParameter,
            UserTypeParameter,
            // ColorParameter,  // TODO: Client side filtering
            DeckTypeParameter,
            // RarityParameter,  // TODO: Client side filtering
            StartDateParameter,
            EndDateParameter,
        ];
    }

    public string Name => "17Lands";

    public List<ICardDataSourceParameter> Parameters { get; init; }

    public List<ICardMetricDescription> MetricDescriptions { get => SeventeenLandsCardData.MetricDescriptions; }

    public ICardDataSource Build()
    {
        return Task.Run(() => BuildAsync(CancellationToken.None)).Result;
    }

    public async Task<ICardDataSource> BuildAsync(CancellationToken cancelation)
    {
        string cacheFilename = BuildCacheFilename();
        Stream? fileStream = await ReadFromCache(cacheFilename, cancelation);
        if (fileStream != null)
        {
            return Build(fileStream);
        }

        Stream? webStream = await ReadFromWeb(cancelation);
        if (webStream != null)
        {
            SaveToCache(webStream, cacheFilename);
            webStream.Position = 0;
            return Build(webStream);
        }

        return new SeventeenLandsCardDataSource();
    }

    private static async Task<Stream?> ReadFromCache(string cacheFilename, CancellationToken cancelation)
    {
        string cachePath = Path.Combine(CacheDirectory, cacheFilename);
        if (!File.Exists(cachePath))
        {
            return null;
        }

        var creationTime = File.GetCreationTime(cachePath);
        var creationTimeSpan = DateTime.Now.Subtract(creationTime);
        if (creationTimeSpan.TotalHours >= CacheValidHours)
        {
            try
            {
                File.Delete(cachePath);
            }
            catch { }
            return null;
        }

        Stream? fileStream = null;
        try
        {

            var data = await File.ReadAllBytesAsync(cachePath, cancelation);
            fileStream = new MemoryStream(data);
        }
        catch (HttpRequestException)
        { }

        return fileStream;
    }

    private static void SaveToCache(Stream stream, string cacheFilename)
    {
        if (!Directory.Exists(CacheDirectory))
        {
            Directory.CreateDirectory(CacheDirectory);
        }

        try 
        {
            using var fs = File.OpenWrite(Path.Combine(CacheDirectory, cacheFilename));
            stream.CopyTo(fs);
        }
        catch { }
    }

    private async Task<Stream?> ReadFromWeb(CancellationToken cancelation)
    {
        var queryParameters = HttpUtility.ParseQueryString(string.Empty);
        if (SetParameter.Value != null)
        {
            queryParameters["expansion"] = SetParameter.Value;
        }

        if (EventTypeParameter.Value != null)
        {
            queryParameters["format"] = EventTypeParameter.Value;
        }

        if (UserTypeParameter.Value != null && UserTypeParameter.Value != ALL_USERS_USER_TYPE)
        {
            queryParameters["user_group"] = UserTypeParameter.Value.ToLower();
        }

        if (DeckTypeParameter.Value != null && DeckTypeParameter.Value != ALL_COLORS_DECK_TYPE)
        {
            queryParameters["colors"] = DeckTypeParameter.Value;
        }

        queryParameters["start_date"] = StartDateParameter.Value.ToString("yyyy-MM-dd");
        queryParameters["end_date"] = EndDateParameter.Value.ToString("yyyy-MM-dd");

        var urlBuilder = new UriBuilder(Url)
        {
            Query = queryParameters.ToString()
        };

        Stream? webStream = null;
        try
        {
            var url = urlBuilder.ToString();
            var data = await httpClient.GetByteArrayAsync(url, cancelation);
            webStream = new MemoryStream(data);
        }
        catch (HttpRequestException)
        { }

        return webStream;
    }

    internal string BuildCacheFilename()
    {
        var sb = new StringBuilder("17Lands_");
        if (SetParameter.Value != null)
        {
            sb.AppendFormat("{0}_", SetParameter.Value);
        }

        if (EventTypeParameter.Value != null)
        {
            sb.AppendFormat("{0}_", EventTypeParameter.Value);
        }

        if (UserTypeParameter.Value != null && UserTypeParameter.Value != ALL_USERS_USER_TYPE)
        {
            sb.AppendFormat("{0}_", UserTypeParameter.Value.ToLower());
        }

        if (DeckTypeParameter.Value != null && DeckTypeParameter.Value != ALL_COLORS_DECK_TYPE)
        {
            sb.AppendFormat("{0}_", DeckTypeParameter.Value.ToLower());
        }

        sb.AppendFormat("{0}_", StartDateParameter.Value.ToString("yyyy-MM-dd"));
        sb.Append(EndDateParameter.Value.ToString("yyyy-MM-dd"));
        sb.Append(".json");

        return sb.ToString();
    }

    internal static SeventeenLandsCardDataSource Build(Stream steam)
    {
        var reader = new StreamReader(steam);
        var data = reader.ReadToEnd();
        return Build(data);
    }

    internal static SeventeenLandsCardDataSource Build(string jsonText)
    {
        var cardData = JsonConvert.DeserializeObject<List<SeventeenLandsCardData>>(jsonText);
        return cardData == null ? throw new ArgumentException("Invalid JSON", nameof(jsonText)) : new SeventeenLandsCardDataSource(cardData);
    }

    private class SeventeenLandFilters
    {
        [JsonProperty("colors")]
        internal List<string?> Colors = [];

        [JsonProperty("expansions")]
        internal List<string> Expansions = [];

        [JsonProperty("formats")]
        internal List<string> Formats = [];

        [JsonProperty("groups")]
        internal List<string?> Groups = [];
    };

    private static void BuildFilters()
    {
        Stream? stream = null;
        try
        {
            var webRequest = new HttpRequestMessage(HttpMethod.Get, FiltersUrl);
            var response = httpClient.Send(webRequest);
            stream = response.Content.ReadAsStream();
        }
        catch (HttpRequestException)
        { }

        if (stream == null)
        {
            throw new InvalidOperationException("Cannot download 17Lands filters");
        }

        var reader = new StreamReader(stream);
        var jsonText = reader.ReadToEnd();
        var filters = JsonConvert.DeserializeObject<SeventeenLandFilters>(jsonText);

        SetList = filters!.Expansions;
        EventTypeList = filters!.Formats;

        UserTypeList = filters!.Groups.Select(x => x == null ? ALL_USERS_USER_TYPE : System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x)).ToList();
        ColorList = filters!.Colors.Select(x => x ?? ALL_COLORS_COLOR_TYPE).ToList();
    }

    private static List<string> SetList = [];
    private static List<string> EventTypeList = [];
    private static List<string> UserTypeList = [];
    private static List<string> ColorList = [];

    private static List<string> GetDeckTypeList()
    {
        return
        [
            ALL_COLORS_DECK_TYPE,
            "W",
            "U",
            "B",
            "R",
            "G",
            "WU",
            "WB",
            "WR",
            "WG",
            "UB",
            "UR",
            "UG",
            "BR",
            "BG",
            "RG",
            "WUB",
            "WUR",
            "WUG",
            "WBR",
            "WBG",
            "WRG",
            "UBR",
            "UBG",
            "URG",
            "BRG",
            "WUBR",
            "WUBG",
            "WURG",
            "WBRG",
            "UBRG",
            "WUBRG",
        ];
    }

    private static List<string> GetRarityList()
    {
        return
        [
            ALL_RARITIES_RARITY_TYPE,
            "Common",
            "Uncommon",
            "Rare",
            "Mythic",
        ];
    }



    private static readonly HttpClient httpClient = new();

    private const string FiltersUrl = "https://www.17lands.com/data/filters";
    private const string Url = "https://www.17lands.com/card_ratings/data";
    private readonly static string AppProgramData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CardPile");
    private readonly static string CacheDirectory = Path.Combine(AppProgramData, "17LandsCache");
    private const int CacheValidHours = 6;

    private const string ALL_USERS_USER_TYPE = "All users";
    private const string ALL_COLORS_COLOR_TYPE = "All colors";
    private const string ALL_COLORS_DECK_TYPE = "All colors";
    private const string ALL_RARITIES_RARITY_TYPE = "All rarities";

    private const string SET_PARAMETER_NAME = "Set";
    private const string EVENT_TYPE_PARAMETER_NAME = "Event type";
    private const string USER_TYPE_PARAMETER_NAME = "User type";
    private const string COLOR_PARAMETER_NAME = "Color";
    private const string DECK_TYPE_PARAMETER_NAME = "Deck type";
    private const string RARITY_PARAMETER_NAME = "Rarity";
    private const string START_DATE_PARAMETER_NAME = "Start date";
    private const string END_DATE_PARAMETER_NAME = "End date";

    private const int StartDataOffsetDays = 21;

    private readonly CardDataSourceParameterOptions SetParameter = new(SET_PARAMETER_NAME, SetList);
    private readonly CardDataSourceParameterOptions EventTypeParameter = new(EVENT_TYPE_PARAMETER_NAME, EventTypeList);
    private readonly CardDataSourceParameterOptions UserTypeParameter = new(USER_TYPE_PARAMETER_NAME, UserTypeList);
    private readonly CardDataSourceParameterOptions ColorParameter = new(COLOR_PARAMETER_NAME, ColorList);
    private readonly CardDataSourceParameterOptions DeckTypeParameter = new(DECK_TYPE_PARAMETER_NAME, GetDeckTypeList());
    private readonly CardDataSourceParameterOptions RarityParameter = new(RARITY_PARAMETER_NAME, GetRarityList());
    private readonly CardDataSourceParameterDate StartDateParameter = new(START_DATE_PARAMETER_NAME, DateTime.Now.AddDays(-StartDataOffsetDays));
    private readonly CardDataSourceParameterDate EndDateParameter = new(END_DATE_PARAMETER_NAME, DateTime.Now);
}
