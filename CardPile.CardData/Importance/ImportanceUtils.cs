namespace CardPile.CardData.Importance;

public class ImportanceUtils
{
    static public string ToMarker(ImportanceLevel level)
    {
        return level switch
        {
            ImportanceLevel.Low => $"{{ImportanceLevel.{ImportanceLevel.Low}}}",
            ImportanceLevel.Regular => $"{{ImportanceLevel.{ImportanceLevel.Regular}}}",
            ImportanceLevel.High => $"{{ImportanceLevel.{ImportanceLevel.High}}}",
            ImportanceLevel.Critical => $"{{ImportanceLevel.{ImportanceLevel.Critical}}}",
            _ => $"{{ImportanceLevel.{ImportanceLevel.Regular}}}",
        };
    }
}
