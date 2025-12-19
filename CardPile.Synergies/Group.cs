using NLog;
using Tomlet;
using Tomlet.Models;

namespace CardPile.Synergies;

public class Group
{
    internal Group(string filepath, string set)
    {
        Set = set;

        this.filepath = filepath;
    }

    public string Set { get; init; }

    internal static Group? TryLoad(string filePath)
    {
        try
        {
            logger.Info("Loading synergies from {filePath}", filePath);

            var document = TomlParser.ParseFile(filePath);

            if (!document.TryGetValue("set", out var setValue))
            {
                logger.Error("Error parsing card group in skeleton. Top level set is missing {document}", document);
                return null;
            }

            var set = setValue.StringValue.ToUpper();

            var result = new Group(filePath, set);

            foreach (var arrayEntry in document.Where(kv => kv.Value is TomlArray))
            {
                if (arrayEntry.Value is not TomlArray array)
                {
                    logger.Error("Error parsing synergies. Array is not a array (duh!) {arrayEntry}", arrayEntry);
                    return null;
                }

                if(array.Count != 2)
                {
                    logger.Error("Error parsing synergies. Synergy entry is not a 2 element array {array}", array);
                    return null;
                }

                var cardNameA = array[0].StringValue;
                var cardNameB = array[1].StringValue;

                result.cardPairs.Add(new CardPair(cardNameA, cardNameB));
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.Error("Error parsing skeleton {filePath}. Exception: {exception}", filePath, ex);
            return null;
        }
    }

    private string filepath = string.Empty;

    private HashSet<CardPair> cardPairs = new HashSet<CardPair>();

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}

