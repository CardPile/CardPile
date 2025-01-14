using CsvHelper;
using System.Globalization;
using CardPile.CardData.Settings;
using CardPile.CardData.SeventeenLands;

namespace CardPile.CardData.Spreadsheet;

public class CardDataSourceBuilder : ICardDataSourceBuilder
{
    public static void Init()
    {
        // NOOP
    }

    public CardDataSourceBuilder()
    {
        Settings = [SpreadSheetFilenameSetting];

        Parameters = [];
    }

    public string Name => "Spreadsheet";

    public List<ICardDataSourceSetting> Settings { get; init; }

    public List<ICardDataSourceParameter> Parameters { get; init; }

    public List<ICardMetricDescription> MetricDescriptions { get => CardData.MetricDescriptions; }

    public ICardDataSource Build()
    {
        return Task.Run(() => BuildAsync(CancellationToken.None)).Result;
    }

    public async Task<ICardDataSource> BuildAsync(CancellationToken cancelation)
    {
        await SaveConfiguration();

        if (!File.Exists(SpreadSheetFilenameSetting.Value))
        {
            return new CardDataSource();
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

        return new CardDataSource(entries);
    }

    private async Task SaveConfiguration()
    {
        Configuration.Instance.Location = SpreadSheetFilenameSetting.Value;

        await Configuration.Instance.Save();
    }

    private SettingPath SpreadSheetFilenameSetting = new SettingPath("Grade file", Configuration.Instance.Location);
}

