using CardPile.Draft;
using CardPile.Parser.Matchers;

namespace CardPile.Parser;

public class DraftPickMatcher : ILogMatcher
{
    public event EventHandler<DraftPickEvent>? DraftPickEvent;

    public bool Match(string line)
    {
        if (!line.StartsWith(DRAFT_PICK_PREFIX_NEEDLE))
        {
            return false;
        }

        if(!line.Contains(DRAFT_PICK_DRAFT_ID_NEEDLE))
        {
            return false;
        }

        var e = ParseDraftEventInfo(line, DRAFT_PICK_PREFIX_NEEDLE);
        if (e == null)
        {
            return false;
        }

        DraftPickEvent?.Invoke(this, e);

        return true;
    }

    private static DraftPickEvent? ParseDraftEventInfo(string line, string needle)
    {
        dynamic? requestData = MatcherHelpers.ParseRequestUnchecked(line, needle);

        var draftId = requestData?.DraftId?.Value;
        var packNumber = requestData?.Pack?.Value;
        var pickNumber = requestData?.Pick?.Value;
        var cardPicked = requestData?.GrpId?.Value;

        if (draftId == null || packNumber == null || pickNumber == null || cardPicked == null ) 
        { 
            return null; 
        }

        return new DraftPickEvent(new Guid(draftId), (int)packNumber, (int)pickNumber, (int)cardPicked);
    }

    private static readonly string DRAFT_PICK_PREFIX_NEEDLE = "[UnityCrossThreadLogger]==> Event_PlayerDraftMakePick";
    private static readonly string DRAFT_PICK_DRAFT_ID_NEEDLE = "DraftId";
}
