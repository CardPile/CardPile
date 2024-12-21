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
                    Logger.Error("Error removing image {cardImageFilePath}. Exception: {exception}", filePath, ex);
                }
            }
        }
    }    
    
    public static async Task<Bitmap?> LoadBitmap(string cacheIdentifier, string? url)
    {
        string cacheDirectory = ScryfallImageCache;
        string cachePath = Path.Combine(cacheDirectory, $"{cacheIdentifier}.jpg");
        
        var (stream, cached) = await GetImageStream(cachePath, cacheIdentifier, url);
        
        Bitmap? bitmap = null;
        if (stream != null)
        {
            bitmap = new Bitmap(stream);

            if (!cached)
            {
                if (!Directory.Exists(cacheDirectory))
                {
                    Directory.CreateDirectory(cacheDirectory);
                }

                await using var fs = File.OpenWrite(cachePath);
                bitmap.Save(fs);
            }
        }

        return bitmap;
    }

    public static async Task<Bitmap?> LoadSvg(string cacheIdentifier, string? url)
    {
        string cacheDirectory = ScryfallImageCache;
        string cachePath = Path.Combine(cacheDirectory, $"{cacheIdentifier}.svg");
        
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
                svg.Picture?.ToSvg(fs, SKColor.Empty, 1.0f, 1.0f);
            }
            
            using var memory = new MemoryStream();
            svg.Picture?.ToImage(
                memory,
                SKColor.Empty,
                SKEncodedImageFormat.Jpeg,
                100,
                1.0f,
                1.0f,
                SKColorType.Bgra8888,
                SKAlphaType.Premul,
                SKColorSpace.CreateSrgb());
            memory.Position = 0;
            bitmap = new Bitmap(memory);
        }
        
        return bitmap;
    }
    
    public static string GetImageUrlFromExpansionAndCollectorNumber(string expansion, string collectorNumber)
    {
        return $"https://api.scryfall.com/cards/{expansion.ToLower()}/{collectorNumber}?format=image";
    }

    private static async Task<(Stream?, bool)> GetImageStream(string cachePath, string cacheIdentifier, string? url)
    {
        Stream? stream = null;
        bool cached = false;
        if (File.Exists(cachePath))
        {
            stream = File.OpenRead(cachePath);
            cached = true;
        }
        else if (url != null)
        {
            var response = await HttpClient.GetAsync(url);
            if(response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                stream = new MemoryStream(data);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync();
                Logger.Warn("Error fetching image for {cacheIdentifier} from {url}", cacheIdentifier, url);
                Logger.Warn("Status: {status} Response: {response}", response.StatusCode, content.Result);
            }
        }

        return (stream, cached);
    }
    
    private static readonly string AppProgramData = OperatingSystem.IsMacOS() ? "/Users/Shared" : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    private static readonly string CardPileProgramData = Path.Combine(AppProgramData, "CardPile");
    private  static readonly string ScryfallImageCache = Path.Combine(CardPileProgramData, "ScryfallImageCache");
    private const int CACHE_VALID_DAYS = 30;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly HttpClient HttpClient = new();
}
