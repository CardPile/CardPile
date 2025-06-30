using Avalonia.Media.Imaging;
using NLog;
using SkiaSharp;
using Svg.Skia;
using Newtonsoft.Json.Linq;

namespace CardPile.CardInfo;

public static class Scryfall
{
    public static readonly string WhiteManaUrl = "https://svgs.scryfall.io/card-symbols/W.svg";
    public static readonly string BlueManaUrl = "https://svgs.scryfall.io/card-symbols/U.svg";
    public static readonly string BlackManaUrl = "https://svgs.scryfall.io/card-symbols/B.svg";
    public static readonly string RedManaUrl = "https://svgs.scryfall.io/card-symbols/R.svg";
    public static readonly string GreenManaUrl = "https://svgs.scryfall.io/card-symbols/G.svg";
    public static readonly string ColorlessManaUrl = "https://svgs.scryfall.io/card-symbols/C.svg";
    
    public static readonly string SetsUrl = "https://api.scryfall.com/sets";
    
    public static readonly Bitmap WhiteManaSymbol; 
    public static readonly Bitmap BlueManaSymbol; 
    public static readonly Bitmap BlackManaSymbol; 
    public static readonly Bitmap RedManaSymbol; 
    public static readonly Bitmap GreenManaSymbol;
    public static readonly Bitmap ColorlessManaSymbol;
    
    static Scryfall()
    {
        HttpClient.DefaultRequestHeaders.Add("User-Agent", "CardPile/0.1");
        HttpClient.DefaultRequestHeaders.Add("Accept", "*/*");

        WhiteManaSymbol = LoadSvg("W", WhiteManaUrl).Result ?? throw new Exception("Could not load white mana image");
        BlueManaSymbol = LoadSvg("U", BlueManaUrl).Result ?? throw new Exception("Could not load blue mana image");
        BlackManaSymbol = LoadSvg("B", BlackManaUrl).Result ?? throw new Exception("Could not load black mana image");
        RedManaSymbol = LoadSvg("R", RedManaUrl).Result ?? throw new Exception("Could not load white red image");
        GreenManaSymbol = LoadSvg("G", GreenManaUrl).Result ?? throw new Exception("Could not load green mana image");
        ColorlessManaSymbol = LoadSvg("C", ColorlessManaUrl).Result ?? throw new Exception("Could not load colorless mana image");

        _ = LoadSets();
    }

    public static void Init()
    {
        // NOOP - runs static constructor
    }
    
    public static void ClearOldImages()
    {
        if (!Directory.Exists(ScryfallImageCache))
        {
            return;
        }

        var filePaths = Directory.GetFiles(ScryfallImageCache);
        foreach (var filePath in filePaths)
        {
            var lastAccessTime = File.GetLastAccessTimeUtc(filePath);
            var lastAccessTimeSpan = DateTime.UtcNow.Subtract(lastAccessTime);
            if (lastAccessTimeSpan.TotalDays >= CacheValidDays)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch(Exception ex)
                {
                    Logger.Error("Error removing image {cardImageFilePath}. Exception: {exception}", filePath, ex);
                }
            }
        }
    }    
    
    public static async Task<Bitmap?> LoadBitmap(string cacheIdentifier, string? url)
    {
        var (cacheDirectory, cachePath) = GetBitmapCachePathFromIdentifier(cacheIdentifier);

        var (stream, cached) = await GetImageStream(cachePath, cacheIdentifier, url);

        if (stream == null)
        {
            return null;
        }

        var bitmap = new Bitmap(stream);

        if (!cached)
        {
            await SaveBitmap(cacheDirectory, cachePath, bitmap);
        }

        return bitmap;
    }

    public static async Task<Bitmap?> LoadBitmapWithSize(string cacheIdentifier, string? url, int width, int height)
    {
        var (cacheDirectory, cachePath) = GetBitmapCachePathFromIdentifierAndWidth(cacheIdentifier, width, height);

        var sizedStream = GetImageStreamFromCache(cachePath);
        if (sizedStream != null)
        {
            return new Bitmap(sizedStream);
        }

        var (fullSizeCacheDirectory, fullSizeCachePath) = GetBitmapCachePathFromIdentifier(cacheIdentifier);
        var (fullSizeStream, fullSizeCached) = await GetImageStream(fullSizeCachePath, cacheIdentifier, url);
        if(fullSizeStream == null)
        {
            return null;
        }

        if (!fullSizeCached)
        {
            await SaveBitmap(fullSizeCacheDirectory, fullSizeCachePath, new Bitmap(fullSizeStream));
            fullSizeStream.Position = 0;
        }

        // Inspired by Avalonia ImmutableBitmap code
        Bitmap bitmap;
        using (var skStream = new SKManagedStream(fullSizeStream))
        using (var skData = SKData.Create(skStream))
        using (var codec = SKCodec.Create(skData))
        {
            // Get the scale that is nearest to what we want (eg: jpg returned 512)
            var supportedScale = codec.GetScaledDimensions((float)width / codec.Info.Width);

            // Decode the bitmap at the nearest size
            var nearest = new SKImageInfo(supportedScale.Width, supportedScale.Height);
            var skBitmap = SKBitmap.Decode(codec, nearest);
            if (skBitmap == null)
            {
                return null;
            }

            if (skBitmap.Width != width || skBitmap.Height != height)
            {
                var scaledBmp = new SKBitmap(width, height);
                skBitmap.ScalePixels(scaledBmp, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
                skBitmap.Dispose();
                skBitmap = scaledBmp;
            }

            using (var skEncodedBitmap = skBitmap.Encode(SKEncodedImageFormat.Jpeg, 100))
            await using (var skImageDataStream = skEncodedBitmap.AsStream())
            {
                bitmap = new Bitmap(skImageDataStream);
            }

            skBitmap.Dispose();
        }

        await SaveBitmap(cacheDirectory, cachePath, bitmap);

        return bitmap;
    }

    public static async Task<Bitmap?> LoadSvg(string cacheIdentifier, string? url)
    {
        var (cacheDirectory, cachePath) = GetSvgCachePathFromIdentifier(cacheIdentifier);
        
        var (stream, cached) = await GetImageStream(cachePath, cacheIdentifier, url);
        
        Bitmap? bitmap = null;
        if (stream != null)
        {
            SKSvg svg = SKSvg.CreateFromStream(stream);
            if (!cached)
            {
                if (!Directory.Exists(cacheDirectory))
                {
                    Directory.CreateDirectory(cacheDirectory);
                }

                await using var fs = File.OpenWrite(cachePath);
                svg.Picture?.ToSvg(fs, SKColors.Transparent, 1.0f, 1.0f);
            }
            
            using var memory = new MemoryStream();
            svg.Picture?.ToImage(
                memory,
                SKColors.Transparent,
                SKEncodedImageFormat.Png,
                100,
                1.0f,
                1.0f,
                SKColorType.Bgra8888,
                SKAlphaType.Premul,
                SKColorSpace.CreateSrgb());
            memory.Seek(0, SeekOrigin.Begin);
            bitmap = new Bitmap(memory);
        }
        
        return bitmap;
    }

    public static string GetImageUrlFromExpansionAndCollectorNumber(string expansion, string collectorNumber)
    {
        return $"https://api.scryfall.com/cards/{expansion.ToLower()}/{collectorNumber}?format=image";
    }

    public static (string, string) GetBitmapCachePathFromIdentifier(string cacheIdentifier)
    {
        string cacheDirectory = ScryfallImageCache;
        string cachePath = Path.Combine(cacheDirectory, $"{cacheIdentifier}.jpg");
        return (cacheDirectory, cachePath);
    }

    public static (string, string) GetBitmapCachePathFromIdentifierAndWidth(string cacheIdentifier, int width, int height)
    {
        string cacheDirectory = ScryfallImageCache;
        string cachePath = Path.Combine(cacheDirectory, $"{cacheIdentifier}_{width}x{height}.jpg");
        return (cacheDirectory, cachePath);
    }

    public static (string, string) GetSvgCachePathFromIdentifier(string cacheIdentifier)
    {
        string cacheDirectory = ScryfallImageCache;
        string cachePath = Path.Combine(cacheDirectory, $"{cacheIdentifier}.svg");
        return (cacheDirectory, cachePath);
    }

    public static List<string> GetSetTree(string setCode)
    {
        List<string> result = [];
        List<string> queue = [setCode];
        while (queue.Count > 0)
        {
            var code = queue.Last();
            queue.RemoveAt(queue.Count - 1);

            result.Add(code);

            var newCodes = Sets
                .Where(s => string.Equals(s.Value.ParentSetCode, code, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Value.Code);
            queue.AddRange(newCodes);
        }

        return result;
    }

    private static async Task SaveBitmap(string directory, string path, Bitmap bitmap)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var fs = File.OpenWrite(path);
        bitmap.Save(fs);
    }

    private static async Task<(Stream?, bool)> GetImageStream(string cachePath, string cacheIdentifier, string? url)
    {
        var stream = GetImageStreamFromCache(cachePath);
        if (stream != null)
        {
            return (stream, true);
        }

        return (await GetStreamFromUrl(cacheIdentifier, url), false);
    }
    
    private static Stream? GetImageStreamFromCache(string cachePath)
    {
        if (!File.Exists(cachePath))
        {
            return null;
        }
        return File.OpenRead(cachePath);
    }
    
    private static async Task LoadSets()
    {
        Sets.Clear();
        
        var url = SetsUrl;
        var moreData = true;
        while (moreData)
        {
            await using var dataStream = await GetStreamFromUrl("sets data", url);
            if (dataStream == null)
            {
                break;
            }
            
            using var reader = new StreamReader(dataStream);
            var jsonText = await reader.ReadToEndAsync();
            dynamic setList = JObject.Parse(jsonText);
            
            var newSets = ParseSetList(setList, url);
            if (newSets != null)
            {
                foreach (var newSet in newSets)
                {
                    Sets.Add(newSet.Code, newSet);
                }
            }
            
            if (!setList.ContainsKey("has_more"))
            {
                Logger.Warn("Error fetching set data from {url}: The 'has_more' property is missing.", url);
                break;
            }
            
            moreData = setList["has_more"];
            if (!moreData)
            {
                break;
            }
            
            if (!setList.ContainsKey("next_page"))
            {
                Logger.Warn("Error fetching set data from {url}: The 'has_more' value was truem but 'next_page' URL is missing.", url);
                break;
            }
            
            url = setList["next_page"];
        }
    }

    private static List<SetInfo>? ParseSetList(dynamic setList, string url)
    {
        if (!setList.ContainsKey("object"))
        {
            Logger.Warn("Error fetching set data from {url}: The response does not contain 'object' property.", url);
            return null;
        }

        if (setList["object"] != "list")
        {
            Logger.Warn("Error fetching set data from {url}: The 'object' property is not equal to 'list'.", url);
            return null;
        }

        if (!setList.ContainsKey("data"))
        {
            Logger.Warn("Error fetching set data from {url}: The 'data' property is missing.", url);
            return null;
        }
            
        dynamic data =  setList["data"];
        if (data is not JArray dataArray)
        {
            Logger.Warn("Error fetching set data from {url}: The 'data' property is not an array.", url);
            return null;
        }

        List<SetInfo> result = [];
        foreach (var entry in dataArray)
        {
            if (entry is not JObject entryObject)
            {
                Logger.Warn("Error fetching set data from {url}: The 'data' array entry is not an object.", url);
                continue;
            }

            if (!entryObject.TryGetValue("code", out var codeValue))
            {
                Logger.Warn("Error fetching set data from {url}: The 'data' array entry object does not contain a 'code' property.", url);
                continue;
            }

            var parentSetCode = string.Empty;
            if (entryObject.TryGetValue("parent_set_code", out var parentSetCodeValue))
            {
                parentSetCode = parentSetCodeValue.ToString();
            }
            
            result.Add(new SetInfo()
            {
                Code = codeValue.ToString(),
                ParentSetCode = parentSetCode,
            });
        }

        return result;
    }
    
    private static async Task<Stream?> GetStreamFromUrl(string identifier, string? url)
    {
        if (url == null)
        {
            return null;
        }

        var response = await HttpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Logger.Warn("Error fetching {identifier} from {url}", identifier, url);
            Logger.Warn("Status: {status} Response: {response}", response.StatusCode, content);
            return null;
        }

        var data = await response.Content.ReadAsByteArrayAsync();
        return new MemoryStream(data);
    }    

    private struct SetInfo
    {
        public string Code;
        public string ParentSetCode;
    };
    
    private static readonly string AppProgramData = OperatingSystem.IsMacOS() ? "/Users/Shared" : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    private static readonly string CardPileProgramData = Path.Combine(AppProgramData, "CardPile");
    private static readonly string ScryfallImageCache = Path.Combine(CardPileProgramData, "ScryfallImageCache");
    private const int CacheValidDays = 30;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly Dictionary<string, SetInfo> Sets = [];
    
    private static readonly HttpClient HttpClient = new();
}
