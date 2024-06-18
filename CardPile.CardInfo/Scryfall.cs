using Newtonsoft.Json;
using System.Data;

namespace CardPile.CardInfo;

public class Scryfall
{
    static Scryfall()
    {
        using Stream? fileStream = ReadFromFile();
        if(fileStream != null)
        {
            LoadFromStream(fileStream);
            return;
        }

        using Stream? webStream = ReadFromWeb();
        if (webStream != null)
        {
            SaveStreamToFile(webStream);
            webStream.Position = 0;
            LoadFromStream(webStream);
            return;
        }
        
        throw new InvalidOperationException("Scryfall: card info not present on disk and web download failed");
    }

    public static void Init()
    {
        // NOOP
    }

    public static string? GetImageUrlFromExpansionAndCollectorNumber(string expansion, string collectorNumber)
    {
        if (expansionAndCollectorNumberToUrl.TryGetValue((expansion.ToLower(), collectorNumber), out string? url))
        {
            return url;
        }
        return null;
    }

    private static Stream? ReadFromFile()
    {
        if (!File.Exists(CachePath))
        {
            return null;
        }

        var creationTime = File.GetCreationTime(CachePath);
        var creationTimeSpan = DateTime.Now.Subtract(creationTime);
        if (creationTimeSpan.TotalHours >= CacheValidHours)
        {
            try
            {
                File.Delete(CachePath);
            }
            catch { }
            return null;
        }

        Stream? stream = null;
        try
        {
            stream = File.OpenRead(CachePath);
        }
        catch { }

        return stream;
    }

    private static Stream? ReadFromWeb()
    {
        string? oracleCardDownloadUri = ReadOracleCardDownloadUri();
        if (oracleCardDownloadUri == null)
        {
            return null;
        }

        return ReadOracleCardData(oracleCardDownloadUri);
    }

    private static string? ReadOracleCardDownloadUri()
    {
        byte[]? data = null;
        try
        {
            data = TheHttpClient.GetByteArrayAsync(Url).Result;
        }
        catch (HttpRequestException)
        { }

        if (data == null)
        {
            return null;
        }

        using Stream? stream = new MemoryStream(data);
        var sr = new StreamReader(stream);
        var cardData = JsonConvert.DeserializeObject<BulkDataRoot>(sr.ReadToEnd());

        return cardData?.Data?.Single(x => x.Type == UniqueArtworkBulkDataType).DownloadUri;
    }

    private static Stream? ReadOracleCardData(string downloadUri)
    {
        byte[]? data = null;
        try
        {
            data = TheHttpClient.GetByteArrayAsync(downloadUri).Result;
        }
        catch (HttpRequestException)
        { }

        if(data == null)
        {
            return null;
        }

        return new MemoryStream(data);
    }

    private static void SaveStreamToFile(Stream stream)
    {
        if (!Directory.Exists(CacheDirectory))
        {
            Directory.CreateDirectory(CacheDirectory);
        }

        try
        {
            using var fs = File.OpenWrite(CachePath);
            stream.CopyTo(fs);
        } catch { }
    }

    private static void LoadFromStream(Stream stream)
    {
        var sr = new StreamReader(stream);
        var cardData = JsonConvert.DeserializeObject<List<OracleCardData>>(sr.ReadToEnd());
        if(cardData ==  null)
        {
            return;
        }

        expansionAndCollectorNumberToUrl = cardData.Where(x => x.Set != null && x.CollectorNumber != null && x.ImageUris != null && x.ImageUris.LargeImageUri != null)
                                                   .GroupBy(x => (x.Set!, x.CollectorNumber!))
                                                   .ToDictionary(x => x.Key, x => x.First().ImageUris!.LargeImageUri!);
    }

    private class BulkData
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("download_uri")]
        public string? DownloadUri { get; set; }
    }

    private class BulkDataRoot
    {
        [JsonProperty("data")]
        public List<BulkData>? Data { get; set; }
    }

    private class OracleCardData
    {
        [JsonProperty("set")]
        public string? Set { get; set; }

        [JsonProperty("collector_number")]
        public string? CollectorNumber { get; set; }

        [JsonProperty("image_uris")]
        public OracleCardDataImageUris? ImageUris { get; set; }
    }

    private class OracleCardDataImageUris
    {
        [JsonProperty("large")]
        public string? LargeImageUri { get; set; }
    }

    private static HttpClient TheHttpClient = new();

    private const string Url = "https://api.scryfall.com/bulk-data";
    private readonly static string AppProgramData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CardPile");
    private readonly static string CacheDirectory = Path.Combine(AppProgramData, "ScryfallCardInfoCache");
    private readonly static string CachePath = Path.Combine(CacheDirectory, "unique_artwork.json");
    private const int CacheValidHours = 12;

    private const string UniqueArtworkBulkDataType = "unique_artwork";

    private static Dictionary<(string, string), string> expansionAndCollectorNumberToUrl = [];
}
