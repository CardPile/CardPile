using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace CardPile.CardInfo;

public class SeventeenLands
{
    static SeventeenLands()
    {
        using Stream? fileStream = ReadFromFile();
        using Stream? webStream = ReadFromWeb();
        if (fileStream != null && webStream != null)
        {
            if (fileStream.Length != webStream.Length)
            {
                SaveStreamToFile(webStream);
                webStream.Position = 0;
                LoadFromStream(webStream);
            }
            else
            {
                LoadFromStream(fileStream);
            }
        }
        else if (fileStream != null && webStream == null)
        {
            LoadFromStream(fileStream);
        }
        else if (fileStream == null && webStream != null)
        {
            SaveStreamToFile(webStream);
            webStream.Position = 0;
            LoadFromStream(webStream);
        }
        else
        {
            throw new InvalidOperationException("17Lands: card info not present on disk and web download failed");
        }
    }

    public static void Init()
    {
        // NOOP
    }

    public static string? GetCardNameFromId(int cardId)
    {
        if (cardIdToName.TryGetValue(cardId, out string? cardName))
        {
            return cardName;
        }
        return null;
    }

    public static int? GetCardIdFromName(string cardName)
    {
        if (cardNameToId.TryGetValue(cardName, out int cardId))
        {
            return cardId;
        }
        return null;
    }

    private static Stream? ReadFromFile()
    {
        Stream? fileStream = null;
        if (File.Exists(CachePath))
        {
            try
            {
                fileStream = File.OpenRead(CachePath);
            }
            catch { }
        }
        return fileStream;
    }

    private static Stream? ReadFromWeb()
    {
        byte[]? data = null;
        try
        {
            data = TheHttpClient.GetByteArrayAsync(Url).Result;
            
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
        }
        catch { }
    }

    private static void LoadFromStream(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<CardInfoEntry>();
        cardIdToName = records.Where(x => x.Name != null).ToDictionary(r => r.ArenaCardId, r => r.Name!);
        cardNameToId = records.Where(x => x.Name != null).ToDictionary(r => r.Name!, r => r.ArenaCardId);
    }

    private class CardInfoEntry
    {
        [Name("id")]
        public int ArenaCardId { get; set; }

        [Name("expansion")]
        public string? Expansion { get; set; }

        [Name("name")]
        public string? Name { get; set; }

        [Name("rarity")]
        public string? Rarity { get; set; }

        [Name("color_identity")]
        public string? ColorIdentity { get; set; }
        
        [Name("mana_value")]
        public int ManaValue { get; set; }

        [Name("types")]
        public string? Types { get; set; }

        [Name("is_booster")]
        public bool IsBooster { get; set; }
    }

    private readonly static HttpClient TheHttpClient = new();

    private const string Url = "https://17lands-public.s3.amazonaws.com/analysis_data/cards/cards.csv";
    private readonly static string AppProgramData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CardPile");
    private readonly static string CacheDirectory = Path.Combine(AppProgramData, "17LandsCardInfoCache");
    private readonly static string CachePath = Path.Combine(CacheDirectory, "cards.csv");

    private static Dictionary<int, string> cardIdToName = [];
    private static Dictionary<string, int> cardNameToId = [];
}
