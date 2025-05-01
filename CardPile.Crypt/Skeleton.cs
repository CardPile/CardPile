using CardPile.CardData.Importance;
using NLog;
using Tomlet.Models;

namespace CardPile.Crypt;

public class Skeleton
{
    internal Skeleton(string name, string set)
    {
        Name = name;
        Set = set;
    }

    public string Name { get; init; }

    public string Set { get; init; }

    public List<CardGroup> Groups { get; } = [];

    public void Update(List<int> cardIds)
    {
        var bones = GetAllBones();

        bones.Sort((lhs, rhs) =>
        {
            var result = -Comparer<ImportanceLevel>.Default.Compare(lhs.Importance, rhs.Importance);
            if(result != 0)
            {
                return result;
            }
            return Comparer<int>.Default.Compare(lhs.Height, rhs.Height);
        });

        bones.ForEach(b => b.ClearCount());

        foreach (var cardId in cardIds)
        {
            foreach (var bone in bones)
            {
                if (bone.TryAddCard(cardId))
                {
                    break;
                }
            }
        }
    }

    internal static Skeleton? TryLoad(TomlDocument document)
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
        var set = setValue.StringValue.ToUpper();

        var skeleton = new Skeleton(name, set);

        foreach (var tableEntry in document.Where(kv => kv.Value is TomlTable))
        {
            if (tableEntry.Value is not TomlTable table)
            {
                logger.Error("Error parsing skeleton. Table is not a table (duh!) {tableEntry}", tableEntry);
                return null;
            }

            var cardGroup = CardGroup.TryLoad(tableEntry.Key, table, set);
            if (cardGroup == null)
            {
                logger.Error("Error parsing skeleton. Invalid card group {table}", table);
                return null;
            }

            if(cardGroup.Groups.Count == 0 && cardGroup.Cards.Count == 0)
            {
                logger.Warn("Card group {cardGroup} is empty.", table);
                continue;
            }

            skeleton.Groups.Add(cardGroup);
        }

        return skeleton;
    }

    internal bool CanBeSatisfied()
    {
        foreach (var group in Groups)
        {
            if(!group.CanBeSatisfied())
            {
                return false;
            }
        }

        return true;
    }

    internal List<IBone> GetAllBones()
    {
        var result = new List<IBone>();
        foreach (var group in Groups)
        {
            result.Add(group);
            result.AddRange(group.GetAllChildBones());
        }
        return result;
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
