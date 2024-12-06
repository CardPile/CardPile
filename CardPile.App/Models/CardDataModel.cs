using Avalonia.Media.Imaging;
using CardPile.App.Services;
using CardPile.CardData;
using NLog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CardPile.App.Models;

internal class CardDataModel : ReactiveObject, ICardDataService
{
    public const int CARD_IMAGE_WIDTH = 172;

    static CardDataModel()
    {
        httpClient.DefaultRequestHeaders.Add("User-Agent", "CardPile/0.1");
        httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
    }

    internal CardDataModel(ICardData cardData)
    {
        this.cardData = cardData;
    }

    internal static void ClearOldCardImages()
    {
        if (!Directory.Exists(ScryfallCardImageCache))
        {
            return;
        }

        var filePaths = Directory.GetFiles(ScryfallCardImageCache);
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
                    logger.Error("Error removing card image {cardImageFilePath}. Exception: {exception}", filePath, ex);
                }
            }
        }
    }


    public string Name
    {
        get => cardData.Name;
    }

    public int ArenaCardId
    {
        get => cardData.ArenaCardId;
    }

    public List<Color> Colors
    {
        get => cardData.Colors;
    }

    public string? Url
    { 
        get => cardData.Url; 
    }

    public List<ICardMetric> Metrics
    { 
        get => cardData.Metrics; 
    }

    public Task<Bitmap?> CardImage
    {
        get => LoadCardImage();
    }

    private async Task<Bitmap?> LoadCardImage()
    {
        string CacheDirectory = ScryfallCardImageCache;
        string CachePath = Path.Combine(CacheDirectory, $"{ArenaCardId}.jpg");

        Stream? stream = null;
        bool cached = false;
        if (File.Exists(CachePath))
        {
            stream = File.OpenRead(CachePath);
            cached = true;
        }
        else if (Url != null)
        {
            var response = await httpClient.GetAsync(Url);
            if(response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                stream = new MemoryStream(data);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync();
                logger.Warn("Error fetching image for car {cardId} from {url}", ArenaCardId, Url);
                logger.Warn("Status: {status} Response: {response}", response.StatusCode, content.Result);
            }
        }

        Bitmap? bitmap = null;
        if (stream != null)
        {
            bitmap = Bitmap.DecodeToWidth(stream, CARD_IMAGE_WIDTH);

            if (!cached)
            {
                if (!Directory.Exists(CacheDirectory))
                {
                    Directory.CreateDirectory(CacheDirectory);
                }

                using (var fs = File.OpenWrite(CachePath))
                {
                    bitmap.Save(fs);
                }
            }
        }

        return bitmap;
    }

    private readonly static string AppProgramData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CardPile");
    private readonly static string ScryfallCardImageCache = Path.Combine(AppProgramData, "ScryfallCardImageCache");
    private const int CacheValidDays = 30;

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private static HttpClient httpClient = new();

    private ICardData cardData;
}
