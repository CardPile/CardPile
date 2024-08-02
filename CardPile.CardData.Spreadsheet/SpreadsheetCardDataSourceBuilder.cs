using CsvHelper;
using System.Globalization;
using NLog;

namespace CardPile.CardData.Spreadsheet;

public class SpreadsheetCardDataSourceBuilder : ICardDataSourceBuilder
{
    public SpreadsheetCardDataSourceBuilder()
    {
        Parameters = [];
    }

    public string Name => "Spreadsheet";

    public List<ICardDataSourceParameter> Parameters { get; init; }

    public List<ICardMetricDescription> MetricDescriptions { get => SpreadsheetCardData.MetricDescriptions; }

    public ICardDataSource Build()
    {
        return Task.Run(() => BuildAsync(CancellationToken.None)).Result;
    }

    public async Task<ICardDataSource> BuildAsync(CancellationToken cancelation)
    {
        if (!File.Exists(SpreadsheetFilename))
        {
            return new SpreadsheetCardDataSource();
        }

        List<SpreadsheetEntry> entries = [];
        using (var reader = new StreamReader(SpreadsheetFilename))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            await foreach(var entry in csv.GetRecordsAsync<SpreadsheetEntry>(cancelation))
            {
                entries.Add(entry);
            }
        }

        return new SpreadsheetCardDataSource(entries);
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly static string ExecutableDirectory = Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".";
    private readonly static string SpreadsheetFilename = Path.Combine(ExecutableDirectory, "Grades.csv");
}

