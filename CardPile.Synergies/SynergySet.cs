using CsvHelper;
using NLog;
using System.Globalization;

namespace CardPile.Synergies;

public class SynergySet
{
    public SynergySet(string set, Dictionary<SynergyKey, Synergy> synergies)
    {
        Set = set;
        Synergies = synergies;
    }

    public string Set { get; init; }

    public Dictionary<SynergyKey, Synergy> Synergies { get; init; }

    public Synergy? GetSynergy(string firstCardName, string secondCardName)
    {
        var key = new SynergyKey(firstCardName, secondCardName);
        return Synergies.GetValueOrDefault(key);
    }

    internal static SynergySet? ParseSynergySet(string filePath)
    {
        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            if (!csv.Read() || !csv.TryGetField<string>(0, out var firstEntry))
            {
                logger.Warn("Cannot read from {filePath}", filePath);
                return null;
            }

            if (string.IsNullOrWhiteSpace(firstEntry))
            {
                logger.Warn("Cannot read from {filePath} - the file does not start with a correct header. It starts with null or whitesapce.", filePath);
                return null;
            }


            if (!firstEntry.StartsWith(CARDPILE_SYNERGIES_NEEDLE))
            {
                logger.Warn("Cannot read from {filePath} - the file does not start with a correct header. It starts with {firstEntry}", filePath, firstEntry);
                return null;
            }

            var setName = firstEntry[CARDPILE_SYNERGIES_NEEDLE.Length..].Trim();

            List<string> cardNames = [];
            for (var i = 1; csv.TryGetField<string>(i, out var cardName); i++)
            {
                if (string.IsNullOrWhiteSpace(cardName))
                {
                    logger.Warn("Cannot read from {filePath} - One of the card headers is null or whitespace.", filePath);
                    break;
                }

                cardNames.Add(cardName);
            }

            if (cardNames.Count + 1 < csv.ColumnCount)
            {
                logger.Warn("Cannot read from {filePath} - the file does not start with a correct header. It starts with {firstEntry}", filePath, firstEntry);
                return null;
            }

            Dictionary<SynergyKey, Synergy> synergies = [];
            while (csv.Read())
            {
                if (!csv.TryGetField<string>(0, out var currentCardName))
                {
                    logger.Warn("Cannot load synergies from {filePath} - cannot read first column of one of the rows", filePath);
                    return null;
                }

                if (string.IsNullOrWhiteSpace(currentCardName))
                {
                    logger.Warn("Cannot load synergies from {filePath} - one of the rows starts with null or whitespace", filePath);
                    return null;
                }

                for (var i = 1; csv.TryGetField<string>(i, out var cardPairSynergy); i++)
                {
                    if (string.IsNullOrWhiteSpace(cardPairSynergy))
                    {
                        continue;
                    }

                    var key = new SynergyKey(currentCardName, cardNames[i - 1]);
                    var synergy = Synergy.ParseSynergy(cardPairSynergy);
                    if (synergy == null)
                    {
                        return null;
                    }

                    if (synergy.Strength == SynergyStrength.None)
                    {
                        continue;
                    }

                    if (synergies.ContainsKey(key))
                    {

                        if (synergies[key].Strength != synergy.Strength ||
                            string.CompareOrdinal(synergies[key].Reason, synergy.Reason) != 0)
                        {
                            logger.Warn("Cannot load synergies from {filePath} - card pair {firstCard} {secondCard} has not reflexive synergy", filePath, key.FirstCard, key.SecondCard);
                            return null;
                        }
                    }

                    synergies[key] = synergy;
                }
            }

            return new SynergySet(setName, synergies);
        }
        catch (Exception ex)
        {
            logger.Error("Error parsing synergy set {filePath}. Exception: {exception}", filePath, ex);
        }

        return null;
    }

    private const string CARDPILE_SYNERGIES_NEEDLE = "Cardpile synergies - ";

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
