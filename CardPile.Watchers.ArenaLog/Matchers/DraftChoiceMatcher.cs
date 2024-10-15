using CardPile.Draft;
using Newtonsoft.Json.Linq;
using NGuid;

namespace CardPile.Watchers.ArenaLog.Matchers;

public class DraftChoiceMatcher : ILogMatcher
{
    public event EventHandler<DraftChoiceEvent>? DraftChoiceEvent;

    public bool Match(string line)
    {
        DraftChoiceEvent? eventInfo = null;
        if (line.StartsWith(PREMIERE_DRAFT_CHOICE_PREFIX_NEEDLE))
        {
            eventInfo = ParsePremiereDraftEventInfo(line, PREMIERE_DRAFT_CHOICE_PREFIX_NEEDLE);
        }
        else if (line.StartsWith(QUICK_DRAFT_CHOICE_PREFIX_NEEDLE))
        {
            eventInfo = ParseQuickDraftEventInfo(line);
        }

        if (eventInfo != null)
        {
            DraftChoiceEvent?.Invoke(this, eventInfo);
            return true;
        }

        return false;
    }

    private DraftChoiceEvent? ParsePremiereDraftEventInfo(string line, string needle)
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

    private DraftChoiceEvent? ParseQuickDraftEventInfo(string line)
    {
        dynamic data = JObject.Parse(line);

        var payloadString = data.Payload?.Value;
        if(payloadString == null)
        {
            return null;
        }

        dynamic payload = JObject.Parse(payloadString);

        var eventName = payload.EventName?.Value;
        var packNumber = payload.PackNumber?.Value;
        var pickNumber = payload.PickNumber?.Value;
        var cardsInPackArray = payload.DraftPack;

        if (eventName == null || packNumber == null || pickNumber == null || cardsInPackArray == null)
        {
            return null;
        }

        var cardsInPackStringArray = cardsInPackArray?.ToObject<List<string>>();
        if(cardsInPackStringArray == null)
        {
            return null;
        }

        var cardsInPack = new List<int>();
        foreach (var cardIdString in cardsInPackStringArray)
        {
            cardsInPack.Add(int.Parse(cardIdString));
        }

        // Generate a name-based guid using the event name since we don't have an actual guid here
        var draftId = GuidHelpers.CreateFromName(MatcherHelpers.CARD_PILE_DRAFT_NAMESPACE_GUID, eventName);

        return new DraftChoiceEvent(draftId, (int)packNumber + 1, (int)pickNumber + 1, cardsInPack);
    }

    private static string PREMIERE_DRAFT_CHOICE_PREFIX_NEEDLE = "[UnityCrossThreadLogger]Draft.Notify";
    private static string QUICK_DRAFT_CHOICE_PREFIX_NEEDLE = "{\"CurrentModule\":\"BotDraft\"";
}
