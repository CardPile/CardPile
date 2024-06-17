using CardPile.Draft;
using CardPile.Parser.Matchers;

namespace CardPile.Parser;

public class DraftEndMatcher : ILogMatcher
{
    public event EventHandler<DraftEndEvent>? DraftEndEvent;

    public bool Match(string line)
    {
        if (!line.StartsWith(PREMIERE_DRAFT_END_NEEDLE))
        {
            return false;
        }
        
        var e = ParseDraftEventInfo(line, PREMIERE_DRAFT_END_NEEDLE);
        if(e == null)
        {
            return false;  
        }

        var draftEndEvent = DraftEndEvent;
        if(draftEndEvent != null)
        {
            draftEndEvent(this, e);
        }

        return true;        
    }

    private DraftEndEvent? ParseDraftEventInfo(string line, string needle)
    {
        dynamic? requestData = MatcherHelpers.ParseRequestUnchecked(line, needle);
        var eventName = requestData?.EventName?.Value;
        if (eventName == null)
        {
            return null;
        }

        if (!eventName.StartsWith(PREMIERE_DRAFT_EVENT_NAME_NEEDLE))
        {
            return null;
        }

        return new DraftEndEvent(eventName);
    }

    private static string PREMIERE_DRAFT_END_NEEDLE = "[UnityCrossThreadLogger]==> Draft_CompleteDraft";
    private static string PREMIERE_DRAFT_EVENT_NAME_NEEDLE = "PremierDraft";
}
