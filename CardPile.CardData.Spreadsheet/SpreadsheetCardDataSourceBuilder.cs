using CsvHelper;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using NLog;
using Newtonsoft.Json;

namespace CardPile.CardData.Spreadsheet;

public class SpreadsheetCardDataSourceBuilder : ICardDataSourceBuilder
{
    public SpreadsheetCardDataSourceBuilder()
    {
        Settings = [SpreadSheetFilenameSetting];

        Parameters = [];
    }

    public string Name => "Spreadsheet";

    public List<ICardDataSourceSetting> Settings { get; init; }

    public List<ICardDataSourceParameter> Parameters { get; init; }

    public List<ICardMetricDescription> MetricDescriptions { get => SpreadsheetCardData.MetricDescriptions; }

    public ICardDataSource Build()
    {
        return Task.Run(() => BuildAsync(CancellationToken.None)).Result;
    }

    public async Task<ICardDataSource> BuildAsync(CancellationToken cancelation)
    {
        await SaveConfiguration();

        if (!File.Exists(SpreadSheetFilenameSetting.Value))
        {
            return new SpreadsheetCardDataSource();
        }

        List<SpreadsheetEntry> entries = [];
        using (var reader = new StreamReader(SpreadSheetFilenameSetting.Value))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            await foreach(var entry in csv.GetRecordsAsync<SpreadsheetEntry>(cancelation))
            {
                entries.Add(entry);
            }
        }

        return new SpreadsheetCardDataSource(entries);
    }

    private async Task SaveConfiguration()
    {
        var config = new ConfigurationBuilder().AddJsonFile(CONFIG_FILENAME, true).Build();
        config[CONFIG_LOCATION_KEY] = SpreadSheetFilenameSetting.Value;
        string json = JsonConvert.SerializeObject(config.AsEnumerable().ToDictionary(x => x.Key, x => x.Value));
        await File.WriteAllTextAsync(CONFIG_FILENAME, json);
    }

    private static string GetSpreadsheetFileName()
    {
        var config = new ConfigurationBuilder().AddJsonFile(CONFIG_FILENAME, true).Build();
        var spreadsheetFilenameFromConfig = config[CONFIG_LOCATION_KEY];
        if(string.IsNullOrEmpty(spreadsheetFilenameFromConfig))
        {
            return Path.Combine(Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".", "Grades.csv");
        }
        else
        {
            return spreadsheetFilenameFromConfig;
        }
    }

    private static readonly string CONFIG_FILENAME = Path.Combine(Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".", "spreadsheet.config.json");

    private static readonly string CONFIG_LOCATION_KEY = "location";

    private CardDataSourceSettingPath SpreadSheetFilenameSetting = new CardDataSourceSettingPath("Grade file", GetSpreadsheetFileName());

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}

