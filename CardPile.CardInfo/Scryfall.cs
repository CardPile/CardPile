using Avalonia.Media.Imaging;
using NLog;
using SkiaSharp;
using Svg.Skia;

namespace CardPile.CardInfo;

public static class Scryfall
{
    public static readonly string WhiteManaUrl = "https://svgs.scryfall.io/card-symbols/W.svg";
    public static readonly string BlueManaUrl = "https://svgs.scryfall.io/card-symbols/U.svg";
    public static readonly string BlackManaUrl = "https://svgs.scryfall.io/card-symbols/B.svg";
    public static readonly string RedManaUrl = "https://svgs.scryfall.io/card-symbols/R.svg";
    public static readonly string GreenManaUrl = "https://svgs.scryfall.io/card-symbols/G.svg";
    public static readonly string ColorlessManaUrl = "https://svgs.scryfall.io/card-symbols/C.svg";
    
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
            if (lastAccessTimeSpan.TotalDays >= CACHE_VALID_DAYS)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch(Exception ex)
                {
                    logger.Error("Error removing image {cardImageFilePath}. Exception: {exception}", filePath, ex);
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
            SaveBitmap(cacheDirectory, cachePath, bitmap);
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
            SaveBitmap(fullSizeCacheDirectory, fullSizeCachePath, new Bitmap(fullSizeStream));
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
            using (var skImageDataStream = skEncodedBitmap.AsStream())
            {
                bitmap = new Bitmap(skImageDataStream);
            }

            skBitmap.Dispose();
        }

        SaveBitmap(cacheDirectory, cachePath, bitmap);

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

    private static async void SaveBitmap(string directory, string path, Bitmap bitmap)
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
        Stream? stream = GetImageStreamFromCache(cachePath);
        if (stream != null)
        {
            return (stream, true);
        }

        return (await GetImageStreamFromUrl(cacheIdentifier, url), false);
    }

    private static async Task<Stream?> GetImageStreamFromUrl(string cacheIdentifier, string? url)
    {
        if (url == null)
        {
            return null;
        }

        var response = await HttpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            logger.Warn("Error fetching image for {cacheIdentifier} from {url}", cacheIdentifier, url);
            logger.Warn("Status: {status} Response: {response}", response.StatusCode, content);
            return null;
        }

        var data = await response.Content.ReadAsByteArrayAsync();
        return new MemoryStream(data);
    }

    private static Stream? GetImageStreamFromCache(string cachePath)
    {
        if (!File.Exists(cachePath))
        {
            return null;
        }
        return File.OpenRead(cachePath);
    }

    private static readonly string AppProgramData = OperatingSystem.IsMacOS() ? "/Users/Shared" : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    private static readonly string CardPileProgramData = Path.Combine(AppProgramData, "CardPile");
    private  static readonly string ScryfallImageCache = Path.Combine(CardPileProgramData, "ScryfallImageCache");
    private const int CACHE_VALID_DAYS = 30;

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private static readonly HttpClient HttpClient = new();
}
