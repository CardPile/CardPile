using CardPile.Draft;
using Newtonsoft.Json.Linq;

namespace CardPile.Parser;

public class DraftChoiceMatcher : ILogMatcher
{
    public event EventHandler<DraftChoiceEvent>? DraftChoiceEvent;

    public bool Match(string line)
    {
        if (!line.StartsWith(PREMIERE_DRAFT_CHOICE_PREFIX_NEEDLE))
        {
            return false;
        }

        var e = ParseDraftEventInfo(line, PREMIERE_DRAFT_CHOICE_PREFIX_NEEDLE);
        if (e == null)
        {
            return false;
        }

        DraftChoiceEvent?.Invoke(this, e);

        return true;
    }

    private DraftChoiceEvent? ParseDraftEventInfo(string line, string needle)
    {
        var index = line.IndexOf(needle, StringComparison.Ordinal);
        var tail = line.Substring(index + needle.Length);

        dynamic data = JObject.Parse(tail);

        var draftId = data.draftId?.Value;
        var packNumber = data.SelfPack?.Value;
        var pickNumber = data.SelfPick?.Value;
        var cardsInPackString = data.PackCards?.Value;

        if (draftId == null || packNumber == null || pickNumber == null || cardsInPackString == null ) 
        { 
            return null; 
        }

        var cardsInPackStringArray = cardsInPackString?.Split(',');
        if(cardsInPackStringArray == null)
        {
            return null;
        }

        var cardsInPack = new List<int>();
        foreach (var cardIdString in cardsInPackStringArray)
        {
            cardsInPack.Add(int.Parse(cardIdString)); 
        }

        return new DraftChoiceEvent(new Guid(draftId), (int)packNumber, (int)pickNumber, cardsInPack);
    }

    private static string PREMIERE_DRAFT_CHOICE_PREFIX_NEEDLE = "[UnityCrossThreadLogger]Draft.Notify";
}
