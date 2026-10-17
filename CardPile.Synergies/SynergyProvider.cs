using CsvHelper;
using NLog;
using System.Globalization;


namespace CardPile.Synergies;

public class SynergyProvider
{
    public SynergyProvider()
    {}

    public Dictionary<string, SynergySet> SynergySets { get; init; } = [];

    public void LoadSynergies(string synergyDirectory)
    {
        if (!Directory.Exists(synergyDirectory))
        {
            return;
        }

        SynergySets.Clear();

        var filePaths = Directory.GetFiles(synergyDirectory, "*.csv", SearchOption.AllDirectories);
        foreach (var filePath in filePaths)
        {
            logger.Info("Attempting to load synergies from {filePath}", filePath);

            var synergySet = SynergySet.ParseSynergySet(filePath);
            if (synergySet == null)
            {
                continue;
            }

            if (!SynergySets.TryAdd(synergySet.Set, synergySet))
            {
                logger.Warn("Duplicate synergy file for set {synergySet} found {filePath}", synergySet.Set, filePath);
            }
        }
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
