using Avalonia.Media.Imaging;
using CardPile.App.Services;
using CardPile.CardData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CardPile.App.Models;

internal class CardDataModel : ReactiveObject, ICardDataService
{
    internal CardDataModel(ICardData cardData)
    {
        this.cardData = cardData;
    }

    public string Name
    {
        get => cardData.Name;
    }

    public int ArenaCardId
    {
        get => cardData.ArenaCardId;
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
        string executableDirectory = Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".";
        string CacheDirectory = Path.Combine(executableDirectory, "ScryfallCardImageCache");
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
            var data = await httpClient.GetByteArrayAsync(Url);
            stream = new MemoryStream(data);
        }

        Bitmap? bitmap = null;
        if (stream != null)
        {
            bitmap = Bitmap.DecodeToWidth(stream, 172);

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

    private static HttpClient httpClient = new();

    private ICardData cardData;
}
