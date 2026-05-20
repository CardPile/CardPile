using Newtonsoft.Json;
using NLog;

namespace CardPile.CardData.SeventeenLands;

public static class CardDEqProvider
{
    internal static async Task<List<RawCardDEq>> LoadDEqDataAsync(CancellationToken cancellation, string? set)
    {
        if (set == null)
        {
            return [];
        }
        
        var cacheFilename = BuildDeqDataCacheFilename(set);
        var fileStream = await ReadFromCache(cacheFilename, cancellation);
        if (fileStream != null)
        {
            return LoadDEqData(fileStream);
        }

        var webStream = await ReadDEqDataFromWeb(cancellation, set);
        if (webStream == null)
        {
            return [];
        }
        
        SaveToCache(webStream, cacheFilename);
        webStream.Position = 0;
        return LoadDEqData(webStream);
    }    
    
    private static List<RawCardDEq> LoadDEqData(Stream steam)
    {
        var reader = new StreamReader(steam);
        var data = reader.ReadToEnd();
        return LoadDEqData(data);
    }
    
    private class DEqData
    {
        [JsonProperty("set_code")]
        internal string SetCode = string.Empty;
        
        [JsonProperty("start_date")]
        internal string StartDate = string.Empty;
        
        [JsonProperty("end_date")]
        internal string EndDate = string.Empty;
        
        [JsonProperty("embargoed")]
        internal bool Embargoed;
        
        [JsonProperty("cards")]
        internal List<RawCardDEq> Cards = [];
    }
    
    private static List<RawCardDEq> LoadDEqData(string jsonText)
    {
        var result = JsonConvert.DeserializeObject<DEqData>(jsonText);
        return result != null ? result.Cards : throw new ArgumentException("Invalid JSON", nameof(jsonText));
    }    
    
    private static string BuildDeqDataCacheFilename(string set)
    {
        return $"DEq_{set}.json";
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

    private static async Task<Stream?> ReadDEqDataFromWeb(CancellationToken cancellation, string set)
    {
        var urlBuilder = new UriBuilder(string.Format(DeqUrlTemplate, set));

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

    private static readonly HttpClient HttpClient = new();
    
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    private const string DeqUrlTemplate = "https://magic-flea.com/on-draft/data/{0}.json";
    private static readonly string AppProgramData = OperatingSystem.IsMacOS() ? "/Users/Shared" : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    private static readonly string CardPileProgramData = Path.Combine(AppProgramData, "CardPile");
    private static readonly string CacheDirectory = Path.Combine(CardPileProgramData, "DEqCache");
    private const int CacheValidHours = 24;
}