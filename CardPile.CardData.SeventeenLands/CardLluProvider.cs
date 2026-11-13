using Newtonsoft.Json;
using NLog;
using System.Text.RegularExpressions;

namespace CardPile.CardData.SeventeenLands;

public static class CardLluProvider
{
    internal static async Task<List<RawCardLluRating>> LoadLluDataAsync(CancellationToken cancellation, string? set)
    {
        if (set == null)
        {
            return [];
        }
        
        var cacheFilename = BuildLluDataCacheFilename(set);
        var fileStream = await ReadFromCache(cacheFilename, cancellation);
        if (fileStream != null)
        {
            return LoadLluData(fileStream);
        }

        var webStream = await ReadLluDataFromWeb(cancellation, set);
        if (webStream == null)
        {
            return [];
        }
        
        SaveToCache(webStream, cacheFilename);
        webStream.Position = 0;
        return LoadLluData(webStream);
    }    
    
    private static List<RawCardLluRating> LoadLluData(Stream steam)
    {
        var reader = new StreamReader(steam);
        var data = reader.ReadToEnd();
        return LoadLluData(data);
    }

    private class LluData
    {
        [JsonProperty("expansion")]
        internal string SetCode = string.Empty;
        
        [JsonProperty("ratings")]
        internal List<RawCardLluRating> Cards = [];
    }
    
    private static List<RawCardLluRating> LoadLluData(string jsonText)
    {
        var result = JsonConvert.DeserializeObject<LluData>(jsonText);
        return result != null ? result.Cards : throw new ArgumentException("Invalid JSON", nameof(jsonText));
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
                    Logger.Error("Error removing card LLU data {filePath}. Exception: {exception}", filePath, ex);
                }
            }
        }
    }    
    
    private static string BuildLluDataCacheFilename(string set)
    {
        return $"LLU_{set}.json";
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

    private static async Task<Stream?> ReadLluDataFromWeb(CancellationToken cancellation, string set)
    {
        // 1. Fetch tier list HTML
        var htmlTierListUrlBuilder = new UriBuilder(string.Format(LluHtmlTierListUrlTemplate, set));

        string tierListHtml = string.Empty;
        try
        {
            var htmlTierListUrl = htmlTierListUrlBuilder.ToString();
            tierListHtml = await HttpClient.GetStringAsync(htmlTierListUrl, cancellation);
        }
        catch (HttpRequestException)
        { }

        // 2. Find JS Bundle in the HTML
        var javaScriptBundleMatches = JavaScriptBundleRegex.Match(tierListHtml);
        if (!javaScriptBundleMatches.Success)
        {
            Logger.Error("The JavaScript bundle was not found in LLU website HTML");
            return null;
        }

        // 3. Fetch the JS bundle
        var javaScriptBundleUrlBuilder = new UriBuilder(string.Format(LluJavaScriptBundleUrlTemplate, javaScriptBundleMatches.Groups[1].Value));

        string tierListJavaScriptBundle = string.Empty;
        try
        {
            var javaScriptBundleUrl = javaScriptBundleUrlBuilder.ToString();
            tierListJavaScriptBundle = await HttpClient.GetStringAsync(javaScriptBundleUrl, cancellation);
        }
        catch (HttpRequestException)
        { }


        // 4. Find the tier list ids
        var mainId = string.Empty;
        foreach (Match obj in MainTierListRegex.Matches(tierListJavaScriptBundle))
        {
            foreach (Match pair in MainTierListPairRegex.Matches(obj.Value))
            {
                if (pair.Groups[1].Value != set)
                {
                    continue;
                }

                mainId = pair.Groups[2].Value;
                break;
            }
                
        }

        // 5. Fetch the ratings
        var tierListUrlBuilder = new UriBuilder(string.Format(LluTierListUrlTemplate, mainId));

        Stream? webStream = null;
        try
        {
            var tierListUrl = tierListUrlBuilder.ToString();
            var data = await HttpClient.GetByteArrayAsync(tierListUrl, cancellation);
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
    
    private const string LluHtmlTierListUrlTemplate = "https://limitedlevelups.com/tier-list/{0}";
    private static readonly Regex JavaScriptBundleRegex = new("""src="(/assets/index-[^"]+\.js)""");

    private const string LluJavaScriptBundleUrlTemplate = "https://limitedlevelups.com{0}";
    private static readonly Regex MainTierListRegex = new("""\{(?:[A-Za-z0-9_]{2,8}:"[0-9a-f]{32}",?){3,}\}""");
    private static readonly Regex MainTierListPairRegex = new("""([A-Za-z0-9_]{2,8}):"([0-9a-f]{32})""");

    private const string LluTierListUrlTemplate = "https://limitedlevelups.com/api/tier-list/{0}";

    private static readonly string AppProgramData = OperatingSystem.IsMacOS() ? "/Users/Shared" : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    private static readonly string CardPileProgramData = Path.Combine(AppProgramData, "CardPile");
    private static readonly string CacheDirectory = Path.Combine(CardPileProgramData, "LLUCache");
    private const int CacheValidHours = 24;
}