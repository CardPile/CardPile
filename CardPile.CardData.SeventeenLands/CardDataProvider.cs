﻿using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NLog;
using System.Text;
using System.Web;

namespace CardPile.CardData.SeventeenLands;

public class SeventeenLandsCardDataSourceProvider
{
    internal const string ALL_USERS_USER_TYPE = "All users";
    internal const string ALL_COLORS_COLOR_TYPE = "All colors";
    internal const string ALL_COLORS_DECK_TYPE = "All colors";
    internal const string ALL_RARITIES_RARITY_TYPE = "All rarities";

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

    internal static List<RawCardData> LoadCardData(string? set, string? eventType, string? userType, string? deckType, DateTime startDate, DateTime endDate)
    {
        return Task.Run(() => LoadCardDataAsync(CancellationToken.None, set, eventType, userType, deckType, startDate, endDate)).Result;
    }

    internal static async Task<List<RawCardData>> LoadCardDataAsync(CancellationToken cancelation, string? set, string? eventType, string? userType, string? deckType, DateTime startDate, DateTime endDate)
    {
        string cacheFilename = BuildCardDataCacheFilename(set, eventType, userType, deckType, startDate, endDate);
        Stream? fileStream = await ReadFromCache(cacheFilename, cancelation);
        if (fileStream != null)
        {
            return LoadCardData(fileStream);
        }

        Stream? webStream = await ReadCardDataFromWeb(cancelation, set, eventType, userType, deckType, startDate, endDate);
        if (webStream != null)
        {
            SaveToCache(webStream, cacheFilename);
            webStream.Position = 0;
            return LoadCardData(webStream);
        }

        return [];
    }

    internal static List<RawCardData> LoadCardData(Stream steam)
    {
        var reader = new StreamReader(steam);
        var data = reader.ReadToEnd();
        return LoadCardData(data);
    }

    internal static List<RawCardData> LoadCardData(string jsonText)
    {
        var result = JsonConvert.DeserializeObject<List<RawCardData>>(jsonText);
        return result == null ? throw new ArgumentException("Invalid JSON", nameof(jsonText)) : result;
    }

    internal static List<RawWinData> LoadWinData(string? set, string? eventType, DateTime startDate, DateTime endDate, bool combineSplashes)
    {
        return Task.Run(() => LoadWinDataAsync(CancellationToken.None, set, eventType, startDate, endDate, combineSplashes)).Result;
    }

    internal static async Task<List<RawWinData>> LoadWinDataAsync(CancellationToken cancelation, string? set, string? eventType, DateTime startDate, DateTime endDate, bool combineSplashes)
    {
        string cacheFilename = BuildWinDataCacheFilename(set, eventType, startDate, endDate, combineSplashes);
        Stream? fileStream = await ReadFromCache(cacheFilename, cancelation);
        if (fileStream != null)
        {
            return LoadWinData(fileStream);
        }

        Stream? webStream = await ReadWinDataFromWeb(cancelation, set, eventType, startDate, endDate, combineSplashes);
        if (webStream != null)
        {
            SaveToCache(webStream, cacheFilename);
            webStream.Position = 0;
            return LoadWinData(webStream);
        }

        return [];
    }

    internal static List<RawWinData> LoadWinData(Stream steam)
    {
        var reader = new StreamReader(steam);
        var data = reader.ReadToEnd();
        return LoadWinData(data);
    }

    internal static List<RawWinData> LoadWinData(string jsonText)
    {
        var result = JsonConvert.DeserializeObject<List<RawWinData>>(jsonText);
        return result == null ? throw new ArgumentException("Invalid JSON", nameof(jsonText)) : result;
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
                    logger.Error("Error removing 17Lands card data {filePath}. Exception: {exception}", filePath, ex);
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
        StartDates = filters!.StartDates;
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

        [JsonProperty("start_dates")]
        internal Dictionary<string, DateTime> StartDates = [];
    };

    private static string BuildCardDataCacheFilename(string? set, string? eventType, string? userType, string? deckType, DateTime startDate, DateTime endDate)
    {
        var sb = new StringBuilder("17Lands_");
        if (set != null)
        {
            sb.AppendFormat("{0}_", set);
        }

        if (eventType != null)
        {
            sb.AppendFormat("{0}_", eventType);
        }

        if (userType != null && userType != ALL_USERS_USER_TYPE)
        {
            sb.AppendFormat("{0}_", userType.ToLower());
        }

        if (deckType != null && deckType != ALL_COLORS_DECK_TYPE)
        {
            sb.AppendFormat("{0}_", deckType.ToLower());
        }

        sb.AppendFormat("{0}_", startDate.ToString("yyyy-MM-dd"));
        sb.Append(endDate.ToString("yyyy-MM-dd"));
        sb.Append(".json");

        return sb.ToString();
    }

    private static async Task<Stream?> ReadFromCache(string cacheFilename, CancellationToken cancelation)
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
                logger.Error("Error deleting {cacheFilePath}. Exception {exception}", cachePath, ex);
            }
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

        sb.AppendFormat("{0}_", startDate.ToString("yyyy-MM-dd"));
        sb.AppendFormat("{0}_", endDate.ToString("yyyy-MM-dd"));
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
        catch { }
    }

    private static async Task<Stream?> ReadCardDataFromWeb(CancellationToken cancelation, string? set, string? eventType, string? userType, string? deckType, DateTime startDate, DateTime endDate)
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
            var data = await httpClient.GetByteArrayAsync(url, cancelation);
            webStream = new MemoryStream(data);
        }
        catch (HttpRequestException)
        { }

        return webStream;
    }

    private static async Task<Stream?> ReadWinDataFromWeb(CancellationToken cancelation, string? set, string? eventType, DateTime startDate, DateTime endDate, bool combineSplashes)
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
            var data = await httpClient.GetByteArrayAsync(url, cancelation);
            webStream = new MemoryStream(data);
        }
        catch (HttpRequestException)
        { }

        return webStream;
    }

    private static readonly HttpClient httpClient = new();

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private const string FiltersUrl = "https://www.17lands.com/data/filters";
    private const string CardDataUrl = "https://www.17lands.com/card_ratings/data";
    private const string WinDataUrl = "https://www.17lands.com/color_ratings/data";
    private readonly static string AppProgramData = OperatingSystem.IsMacOS() ? "/Users/Shared" : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    private readonly static string CardPileProgramData = Path.Combine(AppProgramData, "CardPile");
    private readonly static string CacheDirectory = Path.Combine(CardPileProgramData, "17LandsCache");
    private const int CacheValidHours = 6;
}
