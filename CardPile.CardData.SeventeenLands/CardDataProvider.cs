using Newtonsoft.Json;
using NLog;
using System.Text;
using System.Web;

namespace CardPile.CardData.SeventeenLands;

public class SeventeenLandsThrottlingDelegatingHandler : DelegatingHandler
{
    public SeventeenLandsThrottlingDelegatingHandler(int maxParallelRequests, int maxRequestMillisecondsDelay)
    {
        InnerHandler = new HttpClientHandler();

        semaphore = new SemaphoreSlim(maxParallelRequests);
        rng = new Random();
        maxMillisecondsDelay = maxRequestMillisecondsDelay;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await Task.Delay(rng.Next(0, maxMillisecondsDelay), cancellationToken);

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private readonly SemaphoreSlim semaphore;
    private readonly Random rng;
    private readonly int maxMillisecondsDelay;
}

public static class SeventeenLandsCardDataSourceProvider
{
    static SeventeenLandsCardDataSourceProvider()
    {
        const int MAX_PARALLELISM = 3;
        const int MAX_DELAY_MILLISECONDS = 2000;
        HttpClient = new(new SeventeenLandsThrottlingDelegatingHandler(MAX_PARALLELISM, MAX_DELAY_MILLISECONDS));
    }

    private const string ALL_USERS_USER_TYPE = "All users";
    private const string ALL_COLORS_COLOR_TYPE = "All colors";
    private const string ALL_COLORS_DECK_TYPE = "All colors";
    private const string ALL_RARITIES_RARITY_TYPE = "All rarities";

    internal const string WU_COLORS_DECK_TYPE = "WU";
    internal const string WB_COLORS_DECK_TYPE = "WB";
    internal const string WR_COLORS_DECK_TYPE = "WR";
    internal const string WG_COLORS_DECK_TYPE = "WG";
    internal const string UB_COLORS_DECK_TYPE = "UB";
    internal const string UR_COLORS_DECK_TYPE = "UR";
    internal const string UG_COLORS_DECK_TYPE = "UG";
    internal const string BR_COLORS_DECK_TYPE = "BR";
    internal const string BG_COLORS_DECK_TYPE = "BG";
    internal const string RG_COLORS_DECK_TYPE = "RG";

    internal const string WUB_COLORS_DECK_TYPE = "WUB";
    internal const string WUR_COLORS_DECK_TYPE = "WUR";
    internal const string WUG_COLORS_DECK_TYPE = "WUG";
    internal const string WBR_COLORS_DECK_TYPE = "WBR";
    internal const string WBG_COLORS_DECK_TYPE = "WBG";
    internal const string WRG_COLORS_DECK_TYPE = "WRG";
    internal const string UBR_COLORS_DECK_TYPE = "UBR";
    internal const string UBG_COLORS_DECK_TYPE = "UBG";
    internal const string URG_COLORS_DECK_TYPE = "URG";
    internal const string BRG_COLORS_DECK_TYPE = "BRG";

    internal static async Task<List<RawCardData>> LoadCardDataAsync(CancellationToken cancellation, string? set, string? eventType, string? userType, string? deckType, DateTime startDate, DateTime endDate)
    {
        string cacheFilename = BuildCardDataCacheFilename(set, eventType, userType, deckType, startDate, endDate);
        Stream? fileStream = await ReadFromCache(cacheFilename, cancellation);
        if (fileStream != null)
        {
            return LoadCardData(fileStream);
        }

        Stream? webStream = await ReadCardDataFromWeb(cancellation, set, eventType, userType, deckType, startDate, endDate);
        if (webStream != null)
        {
            SaveToCache(webStream, cacheFilename);
            webStream.Position = 0;
            return LoadCardData(webStream);
        }

        return [];
    }

    private static List<RawCardData> LoadCardData(Stream steam)
    {
        var reader = new StreamReader(steam);
        var data = reader.ReadToEnd();
        return LoadCardData(data);
    }

    private static List<RawCardData> LoadCardData(string jsonText)
    {
        var result = JsonConvert.DeserializeObject<List<RawCardData>>(jsonText);
        return result ?? throw new ArgumentException("Invalid JSON", nameof(jsonText));
    }

    internal static List<RawWinData> LoadWinData(string? set, string? eventType, DateTime startDate, DateTime endDate, bool combineSplashes)
    {
        return Task.Run(() => LoadWinDataAsync(CancellationToken.None, set, eventType, startDate, endDate, combineSplashes)).Result;
    }

    internal static async Task<List<RawWinData>> LoadWinDataAsync(CancellationToken cancellation, string? set, string? eventType, DateTime startDate, DateTime endDate, bool combineSplashes)
    {
        string cacheFilename = BuildWinDataCacheFilename(set, eventType, startDate, endDate, combineSplashes);
        Stream? fileStream = await ReadFromCache(cacheFilename, cancellation);
        if (fileStream != null)
        {
            return LoadWinData(fileStream);
        }

        Stream? webStream = await ReadWinDataFromWeb(cancellation, set, eventType, startDate, endDate, combineSplashes);
        if (webStream != null)
        {
            SaveToCache(webStream, cacheFilename);
            webStream.Position = 0;
            return LoadWinData(webStream);
        }

        return [];
    }

    private static List<RawWinData> LoadWinData(Stream steam)
    {
        var reader = new StreamReader(steam);
        var data = reader.ReadToEnd();
        return LoadWinData(data);
    }

    private static List<RawWinData> LoadWinData(string jsonText)
    {
        List<RawWinData>? result = null;
        try
        {
            result = JsonConvert.DeserializeObject<List<RawWinData>>(jsonText);
        }
        catch(Exception ex)
        {
            Logger.Error("Error deserializing win data. {ex}", ex);
        }

        return result ?? throw new ArgumentException("Invalid JSON", nameof(jsonText));
    }

    internal static void ClearOldData()
    {
        if (!Directory.Exists(CacheDirectory))
        {
            return;
        }

        var filePaths = Directory.GetFiles(CacheDirectory);
        foreach (var filePath in filePaths)
        {
            var lastWriteTime = File.GetLastWriteTimeUtc(filePath);
            var lastWriteTimeSpan = DateTime.UtcNow.Subtract(lastWriteTime);
            if (lastWriteTimeSpan.TotalHours >= CacheValidHours)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Logger.Error("Error removing 17Lands card data {filePath}. Exception: {exception}", filePath, ex);
                }
            }
        }
    }

    internal static List<string> GetDeckTypeList()
    {
        return
        [
            ALL_COLORS_DECK_TYPE,
            "W",
            "U",
            "B",
            "R",
            "G",
            WU_COLORS_DECK_TYPE,
            WB_COLORS_DECK_TYPE,
            WR_COLORS_DECK_TYPE,
            WG_COLORS_DECK_TYPE,
            UB_COLORS_DECK_TYPE,
            UR_COLORS_DECK_TYPE,
            UG_COLORS_DECK_TYPE,
            BR_COLORS_DECK_TYPE,
            BG_COLORS_DECK_TYPE,
            RG_COLORS_DECK_TYPE,
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

    internal static List<string> GetRarityList()
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

    internal static List<string> SetList = [];
    internal static List<string> EventTypeList = [];
    internal static List<string> UserTypeList = [];
    internal static List<string> ColorList = [];
    internal static Dictionary<string, DateTime> StartDates = [];

    internal static void LoadFilters()
    {
        Stream? stream = null;
        try
        {
            var webRequest = new HttpRequestMessage(HttpMethod.Get, FiltersUrl);
            var response = HttpClient.Send(webRequest);
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

        if (filters == null)
        {
            throw new InvalidOperationException("Cannot deserialize 17Lands filters");
        }

        SetList = filters.FilterExpansions;
        EventTypeList = filters.FilterFormats;

        UserTypeList = filters.FilterGroups.Select(x => x == null ? ALL_USERS_USER_TYPE : System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x)).ToList();
        ColorList = filters.FilterColors.Select(x => x ?? ALL_COLORS_COLOR_TYPE).ToList();
        StartDates = filters.FilterStartDates;
    }

    private class SeventeenLandFilters
    {
        [JsonProperty("colors")]
        internal List<string?> FilterColors = [];

        [JsonProperty("expansions")]
        internal List<string> FilterExpansions = [];

        [JsonProperty("formats")]
        internal List<string> FilterFormats = [];

        [JsonProperty("groups")]
        internal List<string?> FilterGroups = [];

        [JsonProperty("start_dates")]
        internal Dictionary<string, DateTime> FilterStartDates = [];
    };

    private static string BuildCardDataCacheFilename(string? set, string? eventType, string? userType, string? deckType, DateTime startDate, DateTime endDate)
    {
        var sb = new StringBuilder("17Lands_");
        if (set != null)
        {
            sb.Append($"{set}_");
        }

        if (eventType != null)
        {
            sb.Append($"{eventType}_");
        }

        if (userType != null && userType != ALL_USERS_USER_TYPE)
        {
            sb.Append($"{userType.ToLower()}_");
        }

        if (deckType != null && deckType != ALL_COLORS_DECK_TYPE)
        {
            sb.Append($"{deckType.ToLower()}_");
        }

        sb.Append($"{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}");
        sb.Append(".json");

        return sb.ToString();
    }

    private static async Task<Stream?> ReadFromCache(string cacheFilename, CancellationToken cancellation)
    {
        string cachePath = Path.Combine(CacheDirectory, cacheFilename);
        if (!File.Exists(cachePath))
        {
            return null;
        }

        var creationTime = File.GetLastWriteTimeUtc(cachePath);
        var creationTimeSpan = DateTime.UtcNow.Subtract(creationTime);
        if (creationTimeSpan.TotalHours >= CacheValidHours)
        {
            try
            {
                File.Delete(cachePath);
            }
            catch(Exception ex)
            {
                Logger.Error("Error deleting {cacheFilePath}. Exception {exception}", cachePath, ex);
            }
            return null;
        }

        Stream? fileStream = null;
        try
        {

            var data = await File.ReadAllBytesAsync(cachePath, cancellation);
            fileStream = new MemoryStream(data);
        }
        catch (HttpRequestException)
        { }

        return fileStream;
    }

    private static string BuildWinDataCacheFilename(string? set, string? eventType, DateTime startDate, DateTime endDate, bool combineSplashes)
    {
        var sb = new StringBuilder("17Lands_winData_");
        if (set != null)
        {
            sb.AppendFormat("{0}_", set);
        }

        if (eventType != null)
        {
            sb.AppendFormat("{0}_", eventType);
        }

        sb.AppendFormat("{0:yyyy-MM-dd}_", startDate);
        sb.AppendFormat("{0:yyyy-MM-dd}_", endDate);
        sb.Append(combineSplashes);
        sb.Append(".json");

        return sb.ToString();
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
        catch
        {
            // Ignored
        }
    }

    private static async Task<Stream?> ReadCardDataFromWeb(CancellationToken cancellation, string? set, string? eventType, string? userType, string? deckType, DateTime startDate, DateTime endDate)
    {
        var queryParameters = HttpUtility.ParseQueryString(string.Empty);
        if (set != null)
        {
            queryParameters["expansion"] = set;
        }

        if (eventType != null)
        {
            queryParameters["format"] = eventType;
        }

        if (userType != null && userType != ALL_USERS_USER_TYPE)
        {
            queryParameters["user_group"] = userType.ToLower();
        }

        if (deckType != null && deckType != ALL_COLORS_DECK_TYPE)
        {
            queryParameters["colors"] = deckType;
        }

        queryParameters["start_date"] = startDate.ToString("yyyy-MM-dd");
        queryParameters["end_date"] = endDate.ToString("yyyy-MM-dd");

        var urlBuilder = new UriBuilder(CardDataUrl)
        {
            Query = queryParameters.ToString()
        };

        Stream? webStream = null;
        try
        {
            var url = urlBuilder.ToString();
            var data = await HttpClient.GetByteArrayAsync(url, cancellation);
            webStream = new MemoryStream(data);
        }
        catch (HttpRequestException)
        { }

        return webStream;
    }

    private static async Task<Stream?> ReadWinDataFromWeb(CancellationToken cancellation, string? set, string? eventType, DateTime startDate, DateTime endDate, bool combineSplashes)
    {
        var queryParameters = HttpUtility.ParseQueryString(string.Empty);
        if (set != null)
        {
            queryParameters["expansion"] = set;
        }

        if (eventType != null)
        {
            queryParameters["event_type"] = eventType;
        }

        queryParameters["start_date"] = startDate.ToString("yyyy-MM-dd");
        queryParameters["end_date"] = endDate.ToString("yyyy-MM-dd");
        queryParameters["combine_splash"] = combineSplashes.ToString().ToLowerInvariant();

        var urlBuilder = new UriBuilder(WinDataUrl)
        {
            Query = queryParameters.ToString()
        };

        Stream? webStream = null;
        try
        {
            var url = urlBuilder.ToString();
            var data = await HttpClient.GetByteArrayAsync(url, cancellation);
            webStream = new MemoryStream(data);
        }
        catch (HttpRequestException)
        { }

        return webStream;
    }

    private static readonly HttpClient HttpClient;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private const string FiltersUrl = "https://www.17lands.com/data/filters";
    private const string CardDataUrl = "https://www.17lands.com/card_ratings/data";
    private const string WinDataUrl = "https://www.17lands.com/color_ratings/data";
    private static readonly string AppProgramData = OperatingSystem.IsMacOS() ? "/Users/Shared" : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    private static readonly string CardPileProgramData = Path.Combine(AppProgramData, "CardPile");
    private static readonly string CacheDirectory = Path.Combine(CardPileProgramData, "17LandsCache");
    private const int CacheValidHours = 24;
}
