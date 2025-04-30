using CardPile.CardData.Importance;
using CardPile.CardInfo;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NLog;
using System;
using System.Data;
using Tomlet;
using Tomlet.Models;

namespace CardPile.Crypt;

public class Crypt
{
    public Crypt()
    {
        LoadSkeletons();
    }

    public List<Skeleton> Skeletons { get; init; } = [];

    private void LoadSkeletons()
    {
        string executableDirectory = Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) ?? "." : ".";
        string skeletonDirectory = Path.Combine(executableDirectory, "Skeletons");

        if (!Directory.Exists(skeletonDirectory))
        {
            return;
        }

        var filePaths = Directory.GetFiles(skeletonDirectory, "*.toml", SearchOption.AllDirectories);
        foreach (var filePath in filePaths)
        {
            try
            {
                var document = TomlParser.ParseFile(filePath);
                var skeleton = TryLoadSkeleton(document);
                if (skeleton == null)
                {
                    logger.Error("Error parsing skeleton file {filePath}", filePath);
                    continue;
                }

                if(!skeleton.CanBeSatisfied())
                {
                    logger.Warn("Skeleton {name} at {filePath} cannot be satisfied.", skeleton.Name, filePath);
                }

                Skeletons.Add(skeleton);
            }
            catch (Exception ex)
            {
                logger.Error("Error parsing skeleton {filePath}. Exception: {exception}", filePath, ex);
            }
        }
    }

    private Skeleton? TryLoadSkeleton(TomlDocument document)
    {
        if (!document.TryGetValue("name", out var nameValue))
        {
            logger.Error("Error parsing card group in skeleton. Top level name is missing {document}", document);
            return null;
        }

        if (!document.TryGetValue("set", out var setValue))
        {
            logger.Error("Error parsing card group in skeleton. Top level set is missing {document}", document);
            return null;
        }

        var name = nameValue.StringValue;
        var set = setValue.StringValue;

        var skeleton = new Skeleton(name, set);

        foreach (var tableEntry in document.Where(kv => kv.Value is TomlTable))
        {
            if (tableEntry.Value is not TomlTable table)
            {
                logger.Error("Error parsing skeleton. Table is not a table (duh!) {tableEntry}", tableEntry);
                return null;
            }

            var cardGroup = TryParseCardGroup(tableEntry.Key, table, set);
            if (cardGroup == null)
            {
                logger.Error("Error parsing skeleton. Invalid card group {table}", table);
                return null;
            }

            skeleton.Groups.Add(cardGroup);
        }

        return skeleton;
    }

    private CardGroup? TryParseCardGroup(string name, TomlTable table, string set)
    {
        Range? totalRange = null;
        if(table.TryGetValue("total", out var totalValue))
        {
            var totalRangeString = totalValue.StringValue;
            var parsedTotalRange = TryParseRange(totalRangeString);
            if(parsedTotalRange == null)
            {
                logger.Error("Error parsing card group in skeleton. Invalid total range {totalRangeString}", totalRangeString);
                return null;
            }

            totalRange = parsedTotalRange;
        }

        var importance = ImportanceLevel.Regular;
        if(table.TryGetValue("importance", out var importanceValue))
        {
            var importanceString = importanceValue.StringValue;
            if (!Enum.TryParse(importanceString, true, out importance))
            {
                logger.Error("Error parsing card group in skeleton. Invalid importance {importanceString}", importanceString);
                return null;
            }
        }

        var group = new CardGroup(name, totalRange, importance);

        if(table.TryGetValue("cards", out var cardsArrayValue) && cardsArrayValue is TomlArray cardsArray)
        {
            foreach (var cardArrayEntry in cardsArray)
            {
                if (cardArrayEntry is not TomlArray card)
                {
                    logger.Error("Error parsing card group in skeleton. Card entry is not an array {cardEntry}", cardArrayEntry);
                    return null;
                }

                var parsedCard = TryParseCardEntry(card, set);
                if(parsedCard == null)
                {
                    logger.Error("Error parsing card group in skeleton. Invalid card entry {card}", card);
                    return null;
                }

                group.Cards.Add(parsedCard);                
            }

            group.Cards.Sort((lhs, rhs) => -Comparer<ImportanceLevel>.Default.Compare(lhs.Importance, rhs.Importance));
        }

        foreach (var subtableEntry in table.Where(kv => kv.Value is TomlTable))
        {
            if (subtableEntry.Value is not TomlTable subtable)
            {
                logger.Error("Error parsing card group in skeleton. Subtable is not a table {subtableEntry}", subtableEntry);
                return null;
            }

            var cardGroup = TryParseCardGroup(subtableEntry.Key, subtable, set);
            if (cardGroup == null)
            {
                logger.Error("Error parsing card group in skeleton. Invalid subtable {subtable}", subtable);
                return null;
            }

            group.Groups.Add(cardGroup);
        }

        return group;
    }

    private static CardEntry? TryParseCardEntry(TomlArray card, string set)
    {
        if(card.Count < 2 && 3 < card.Count)
        {
            logger.Error("Error parsing card entry in skeleton. Too many entries in card description {card}", card);
            return null;
        }

        var cardName = card.ArrayValues[0].StringValue.Trim();
        if(string.IsNullOrEmpty(cardName))
        {
            logger.Error("Error parsing card entry in skeleton. Invalid card name {cardName}", cardName);
            return null;
        }

        var cardId = Arena.GetFirstCardIdsFromNameAndExpansion(cardName, set);
        if (cardId == null)
        {
            logger.Error("Error parsing card entry in skeleton. Unknown card name {cardName}", cardName);
            return null;
        }

        var rangeString = card.ArrayValues[1].StringValue;
        var range = TryParseRange(rangeString);
        if (range == null)
        {
            logger.Error("Error parsing card entry in skeleton. Invalid range {rangeString}", rangeString);
            return null;
        }

        var importance = ImportanceLevel.Regular;
        if(card.Count == 3)
        {
            var importanceString = card.ArrayValues[2].StringValue;
            if (!Enum.TryParse(importanceString, true, out importance))
            {
                logger.Error("Error parsing card entry in skeleton. Invalid importance {importanceString}", importanceString);
                return null;
            }
        }

        return new CardEntry(cardName, cardId.Value, range, importance);
    }

    private static Range? TryParseRange(string str)
    {
        if (str.EndsWith('+'))
        {
            if (int.TryParse(str.AsSpan(0, str.Length - 1), out int from))
            {
                return new Range { From = from, To = int.MaxValue };
            }
        }
        else if (str.EndsWith('-'))
        {
            if (int.TryParse(str.AsSpan(0, str.Length - 1), out int to))
            {
                return new Range { From = 0, To = to };
            }
        }
        else if(str.Contains('-'))
        {
            var parts = str.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && int.TryParse(parts[0], out int rangeFrom) && int.TryParse(parts[1], out int rangeTo))
            {
                return new Range { From = rangeFrom, To = rangeTo };
            }
        }
        else
        {
            if(int.TryParse(str, out int value))
            {
                return new Range { From = value, To = value };
            }
        }

        return null;
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
